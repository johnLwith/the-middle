using WebAppp.Models;

namespace WebAppp.Services
{
    public interface INlpService
    {
        Task<NlpAnalysisResponse> AnalyzeTextAsync(string text);
    }
}