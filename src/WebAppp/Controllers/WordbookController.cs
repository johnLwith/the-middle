using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAppp.Models;
using WebAppp.Services;

namespace WebAppp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordbookController : ControllerBase
    {
        private readonly IWordbookService _wordbookService;

        public WordbookController(IWordbookService wordbookService)
        {
            _wordbookService = wordbookService;
        }

        [HttpPost]
        public async Task<IActionResult> AddWord([FromBody] AddWordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Word) || string.IsNullOrWhiteSpace(request.EpisodeId))
            {
                return BadRequest("Word and EpisodeId are required");
            }

            try
            {
                var word = await _wordbookService.AddWordAsync(request.Word, request.EpisodeId);
                return Created();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the word: {ex.Message}");
            }
        }

        [HttpGet("{episodeId}")]
        public async Task<IActionResult> GetWordsByEpisodeId(string episodeId)
        {
            if (string.IsNullOrWhiteSpace(episodeId))
            {
                return BadRequest("EpisodeId is required");
            }

            try
            {
                var words = await _wordbookService.GetWordsByEpisodeIdAsync(episodeId);
                return Ok(words);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching words: {ex.Message}");
            }
        }
    }

    public class AddWordRequest
    {
        public string Word { get; set; }
        public string EpisodeId { get; set; }
    }
}