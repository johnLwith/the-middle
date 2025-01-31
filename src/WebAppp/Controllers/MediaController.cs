using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using WebAppp.Models;

namespace WebAppp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public MediaController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("subtitles/{episode}")]
        public async Task<ActionResult<IEnumerable<SubtitleItem>>> GetSubtitles(string episode)
        {
            var srtPath = Path.Combine(_environment.WebRootPath, "data", "srt", $"{episode}.srt");
            
            if (!System.IO.File.Exists(srtPath))
                return NotFound();

            var subtitles = new List<SubtitleItem>();
            var lines = await System.IO.File.ReadAllLinesAsync(srtPath);
            
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

                var textBuilder = new StringBuilder();
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    textBuilder.AppendLine(lines[i++]);
                }
                subtitle.Text = textBuilder.ToString().Trim();

                subtitles.Add(subtitle);
            }

            return Ok(subtitles);
        }

        [HttpGet("audio/{episode}")]
        public IActionResult GetAudio(string episode)
        {
            var audioPath = Path.Combine(_environment.WebRootPath, "data", "mp3", $"{episode}.mp3");
            
            if (!System.IO.File.Exists(audioPath))
                return NotFound();

            // To support audio player seeking.
            Response.Headers.Append("Accept-Ranges", "bytes");
            return PhysicalFile(audioPath, "audio/mpeg");
        }

        [HttpGet("episodes")]
        public IActionResult GetEpisodes()
        {
            var srtPath = Path.Combine(_environment.WebRootPath, "data", "srt");
            var mp3Path = Path.Combine(_environment.WebRootPath, "data", "mp3");

            if (!Directory.Exists(srtPath) || !Directory.Exists(mp3Path))
                return NotFound();

            var episodes = Directory.GetFiles(srtPath, "*.srt")
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .Where(episode => System.IO.File.Exists(Path.Combine(mp3Path, $"{episode}.mp3")))
                .OrderBy(episode => episode)
                .ToList();

            return Ok(episodes);
        }
    }
} 