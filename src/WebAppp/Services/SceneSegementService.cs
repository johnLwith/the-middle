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

        public async Task<List<SegementSubtitleModel>> SegementAsync(string episodeId, bool useCache)
        {
            string data = null;
            if (useCache)
            {
                data = GetPilotSegments();
            }
            else
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
                data = result.ToString()[8..^4];
            }
            return JsonSerializer.Deserialize<List<SegementSubtitleModel>>(data);
        }

        private string GetPilotSegments()
        {
            return """
                        [
                {
                    "segments": "1-3",
                    "description": "Opening credits and episode title."
                },
                {
                    "segments": "4-7",
                    "description": "Frankie tries to communicate on the phone while introducing the setting."
                },
                {
                    "segments": "8-13",
                    "description": "Frankie describes Orson, Indiana, and its quirks."
                },
                {
                    "segments": "14-20",
                    "description": "Frankie reflects on her life and family while getting the kids ready for school."
                },
                {
                    "segments": "21-32",
                    "description": "Brick's quirks and morning routine."
                },
                {
                    "segments": "33-42",
                    "description": "Family chaos and Axel's sarcastic comments."
                },
                {
                    "segments": "43-50",
                    "description": "Frankie compares her old and new driver's license photos."
                },
                {
                    "segments": "51-63",
                    "description": "Sue announces her audition for show choir."
                },
                {
                    "segments": "64-77",
                    "description": "Family dynamics and morning rush."
                },
                {
                    "segments": "78-83",
                    "description": "Frankie reflects on her life being drained by responsibilities."
                },
                {
                    "segments": "84-87",
                    "description": "Frankie and Mike's jobs are introduced."
                },
                {
                    "segments": "88-102",
                    "description": "Frankie's struggles at work and low paycheck."
                },
                {
                    "segments": "103-117",
                    "description": "Frankie tries to sell a car but gets interrupted by a call from Brick's school."
                },
                {
                    "segments": "118-136",
                    "description": "Frankie picks up Brick and tries to sell the car to Gail."
                },
                {
                    "segments": "137-153",
                    "description": "Family dinner and Sue's show choir audition."
                },
                {
                    "segments": "154-166",
                    "description": "Brick announces a teacher meeting, and family discussions."
                },
                {
                    "segments": "167-182",
                    "description": "Mike talks about work, and Sue seeks approval for show choir."
                },
                {
                    "segments": "183-212",
                    "description": "Meeting with Brick's teacher about his quirks."
                },
                {
                    "segments": "213-234",
                    "description": "Frankie's work struggles and family responsibilities clash."
                },
                {
                    "segments": "235-250",
                    "description": "Axel's suspension and family tension."
                },
                {
                    "segments": "251-267",
                    "description": "Brick's book report and family dynamics."
                },
                {
                    "segments": "268-285",
                    "description": "Sue announces she made show choir, and family celebrates."
                },
                {
                    "segments": "286-311",
                    "description": "Show choir performance reveal and Sue's role."
                },
                {
                    "segments": "312-326",
                    "description": "Frankie's struggles and Brick's book report mix-up."
                },
                {
                    "segments": "327-356",
                    "description": "Frankie bonds with Gail over motherhood and sells a car."
                },
                {
                    "segments": "357-374",
                    "description": "Frankie and Gail's joyride and escape."
                },
                {
                    "segments": "375-392",
                    "description": "Family reconciliation and Frankie's reflection on her life."
                },
                {
                    "segments": "393-404",
                    "description": "Closing scenes and credits."
                }
            ]
            """;
        }
    }
}
