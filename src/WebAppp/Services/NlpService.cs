using System.Text.Json;
using WebAppp.Models;

namespace WebAppp.Services
{
    public class NlpService : INlpService
    {
        private readonly HttpClient _httpClient;
        private readonly string _nlpApiBaseUrl;

        public NlpService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _nlpApiBaseUrl = configuration["NlpApiBaseUrl"] ?? "http://localhost:5000";
        }

        public async Task<NlpAnalysisResponse> AnalyzeTextAsync(string text)
        {
            var request = new NlpAnalysisRequest { Text = text };
            var response = await _httpClient.PostAsJsonAsync($"{_nlpApiBaseUrl}/analyze", request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<NlpAnalysisResponse>();
            return result ?? new NlpAnalysisResponse { Text = text };
        }
    }
}