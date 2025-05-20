using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using WebApp.Models;
using WebApp.Services;
using WebAppp.Data;
using WebAppp.Models;
using WebAppp.Services;

namespace WebAppp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EpisodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly HttpClient _httpClient;
        private readonly INlpService _nlpService;
        private readonly ISceneSegementService _sceneSegementService;

        public EpisodesController(ApplicationDbContext context, IWebHostEnvironment environment, 
            HttpClient httpClient, INlpService nlpService, ISceneSegementService sceneSegementService)
        {
            _context = context;
            _environment = environment;
            _httpClient = httpClient;
            _nlpService = nlpService;
            _sceneSegementService = sceneSegementService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Episode>>> GetEpisodes()
        {
            return await _context.Episodes
                .OrderBy(e => e.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Episode>> GetEpisode(string id)
        {
            var episode = await _context.Episodes.FindAsync(id);

            if (episode == null)
            {
                return NotFound();
            }

            return episode;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Episode>> CreateEpisode(Episode episode)
        {
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEpisode(string id, Episode episode)
        {
            if (id != episode.Id)
            {
                return BadRequest();
            }

            _context.Entry(episode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EpisodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool EpisodeExists(string id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}/subtitles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SubtitleItem>>> GetSubtitles(string id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null || string.IsNullOrEmpty(episode.SubtitlePath))
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync(episode.SubtitlePath);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            List<SubtitleItem> subtitles = await GetSutitleItems(response);

            return Ok(subtitles);
        }

        private static async Task<List<SubtitleItem>> GetSutitleItems(HttpResponseMessage response)
        {
            var subtitleContent = await response.Content.ReadAsStringAsync();
            var subtitles = new List<SubtitleItem>();
            var lines = subtitleContent.Split(Environment.NewLine);

            for (int i = 0; i < lines.Length;)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    i++;
                    continue;
                }

                var subtitle = new SubtitleItem
                {
                    Id = int.Parse(lines[i++])
                };

                if (i >= lines.Length) break;

                var times = lines[i++].Split(" --> ");
                subtitle.StartTime = TimeSpan.Parse(times[0].Replace(',', '.'));
                subtitle.EndTime = TimeSpan.Parse(times[1].Replace(',', '.'));

                var textBuilder = new System.Text.StringBuilder();
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    textBuilder.AppendLine(lines[i++]);
                }
                subtitle.Text = textBuilder.ToString().Trim();

                subtitles.Add(subtitle);
            }

            return subtitles;
        }

        [HttpGet("analyze")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NlpAnalysisResponse>> AnalyzeSubtitle([MaxLength(100)]string text)
        {
           var analysis = await _nlpService.AnalyzeTextAsync(text);
           return Ok(analysis);
        }

        [HttpPost("segement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<SegementSubtitleModel>>> SegementSubtitle(string id)
        {
            var segements = await _sceneSegementService.SegementAsync(id, true);
            return Ok(segements);
        }
    }
}