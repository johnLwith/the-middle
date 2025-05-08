using Microsoft.AspNetCore.Mvc;
using WebAppp.Services;

namespace WebAppp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly ITranslateService _translateService;

        public TranslateController(ITranslateService translateService)
        {
            _translateService = translateService;
        }

        [HttpGet]
        public async Task<IActionResult> Translate([FromQuery] string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return BadRequest("Word parameter is required");
            }

            try
            {
                var translation = await _translateService.TranslateWordAsync(word);
                return Ok(new { translation = translation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while translating: {ex.Message}");
            }
        }
    }
}