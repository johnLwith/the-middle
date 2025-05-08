using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace WebAppp.Services
{
    public class DeepseekTranslateService : ITranslateService
    {
        private readonly IConfiguration _configuration;
        private readonly Kernel _kernel;

        public DeepseekTranslateService(IConfiguration configuration)
        {
            _configuration = configuration;
            var kernelBuilder = Kernel.CreateBuilder();

#pragma warning disable SKEXP0010
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: configuration["DeepSeek:ModelId"], // Optional name of the underlying model if the deployment name doesn't match the model name
                endpoint: new Uri(configuration["DeepSeek:Endpoint"]),
                apiKey: configuration["DeepSeek:ApiKey"]
            );
#pragma warning restore SKEXP0010

            _kernel = kernelBuilder.Build();
        }

        public async Task<string> TranslateWordAsync(string word)
        {
            var prompt = $"Translate the following English word to Chinese: {word}. Only return the Chinese translation without any explanation or additional text.";
            var result = await _kernel.InvokePromptAsync(prompt);
            return result.ToString().Trim();
        }
    }
}