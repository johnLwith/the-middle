using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppp.Data;
using WebAppp.Models;
using System.Text.Json;
using System.Net.Http;
using System.Linq;
using WebAppp.Services;
using System.Collections.Generic;

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

        public EpisodesController(ApplicationDbContext context, IWebHostEnvironment environment, HttpClient httpClient, INlpService nlpService)
        {
            _context = context;
            _environment = environment;
            _httpClient = httpClient;
            _nlpService = nlpService;
        }

        /// <summary>
        /// Gets all episodes
        /// </summary>
        /// <returns>List of all episodes</returns>
        /// <response code="200">Returns the list of episodes</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Episode>>> GetEpisodes()
        {
            return await _context.Episodes
                .OrderBy(e => e.SeasonNumber)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a specific episode by ID
        /// </summary>
        /// <param name="id">The episode ID</param>
        /// <returns>The requested episode</returns>
        /// <response code="200">Returns the requested episode</response>
        /// <response code="404">If the episode is not found</response>
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

        /// <summary>
        /// Creates a new episode
        /// </summary>
        /// <param name="episode">The episode to create</param>
        /// <returns>The created episode</returns>
        /// <response code="201">Returns the newly created episode</response>
        /// <response code="400">If the episode data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Episode>> CreateEpisode(Episode episode)
        {
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEpisode), new { id = episode.Id }, episode);
        }

        /// <summary>
        /// Updates an existing episode
        /// </summary>
        /// <param name="id">The episode ID</param>
        /// <param name="episode">The updated episode data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the episode was successfully updated</response>
        /// <response code="400">If the episode data is invalid</response>
        /// <response code="404">If the episode is not found</response>
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

        /// <summary>
        /// Gets subtitles for a specific episode
        /// </summary>
        /// <param name="id">The episode ID</param>
        /// <returns>The episode's subtitles</returns>
        /// <response code="200">Returns the episode's subtitles</response>
        /// <response code="404">If the episode or subtitles are not found</response>
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

        /// <summary>
        /// Analyzes the text of a subtitle using NLP for POS tagging
        /// </summary>
        /// <param name="id">The subtitle ID</param>
        /// <returns>NLP analysis results including POS tags</returns>
        /// <response code="200">Returns the NLP analysis results</response>
        /// <response code="404">If the subtitle is not found</response>
        [HttpGet("{id}/analyze")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NlpAnalysisResponse>> AnalyzeSubtitle(string id)
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
           var subtitleContent = subtitles.Select(s => s.Text).Aggregate((a, b) => a + " " + b);
           var analysis = await _nlpService.AnalyzeTextAsync(subtitleContent);
           return Ok(analysis);
        }
    }
}