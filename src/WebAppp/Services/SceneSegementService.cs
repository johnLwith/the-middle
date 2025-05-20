using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.Net.Http;
using System.Text.Json;
using WebApp.Models;
using WebAppp.Data;

namespace WebApp.Services
{
    public class SceneSegementService : ISceneSegementService
    {
        private readonly Kernel _kernel;
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public SceneSegementService(Kernel kernel, ApplicationDbContext dbContext, HttpClient httpClient)
        {
            this._kernel = kernel;
            this._dbContext = dbContext;
            this._httpClient = httpClient;
        }

        public async Task<List<SegementSubtitleModel>> SegementAsync(string episodeId)
        {
            var episode = await _dbContext.Episodes.FindAsync(episodeId);
            var subtitle = await _httpClient.GetStringAsync(episode.SubtitlePath);

            var prompt = """
                # Instrument
                Segement tv show input subtitles by sence.
                
                
                # Output 
                Only return json response, DO NOT include comments or other extra data. example format: 
                `
                [ { "segements":"1-20", "description": "Brick has trouble relating to his teacher with odd results."} ]
                `

                # Input
                `
                {{$subtitle}}
                `
                """;
            var result = await _kernel.InvokePromptAsync(prompt, new KernelArguments {
               { "subtitle", subtitle }
            });
            var data = result.ToString()[8..^4];
            return JsonSerializer.Deserialize<List<SegementSubtitleModel>>(data);
        }
    }
}
