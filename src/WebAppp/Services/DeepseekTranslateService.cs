using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace WebAppp.Services
{
    public class DeepseekTranslateService : ITranslateService
    {
        private readonly Kernel _kernel;

        public DeepseekTranslateService(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<string> TranslateWordAsync(string word)
        {
            var prompt = $"Translate the following English word to Chinese: {word}. Only return the Chinese translation without any explanation or additional text.";
            var result = await _kernel.InvokePromptAsync(prompt);
            return result.ToString().Trim();
        }
    }
}