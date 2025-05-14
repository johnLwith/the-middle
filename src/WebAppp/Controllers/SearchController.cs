using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppp.Models;
using WebAppp.Services;

namespace WebAppp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;

        public SearchController(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateEmbeddings()
        {
            await _embeddingService.StoreEmbeddings();
            return Ok(new { message = "Embeddings generated and stored successfully" });
        }

        [HttpGet("embeddings")]
        public async Task<ActionResult<List<SearchResult>>> SearchEmbeddings([MaxLength(100)][FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { error = "Query parameter is required" });
            }

            var results = await _embeddingService.SearchEmbeddings(query);
            return Ok(results);
        }

        [HttpGet("content")]
        public async Task<ActionResult<List<SearchResult>>> SearchContent([MaxLength(100)][FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { error = "Query parameter is required" });
            }

            var results = await _embeddingService.SearchContent(query);
            return Ok(results);
        }

    }
}