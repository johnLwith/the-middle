using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppp.Services;

namespace WebAppp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmbeddingsController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;

        public EmbeddingsController(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateEmbeddings()
        {
            await _embeddingService.StoreEmbeddings();
            return Ok(new { message = "Embeddings generated and stored successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> SearchEmbeddings([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new { error = "Query parameter is required" });
            }

            var results = await _embeddingService.SearchEmbeddings(query);
            return Ok(results);
        }
    }
}