namespace WebAppp.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001
public class OllamaTextEmbeddingGeneration
{
    private ITextEmbeddingGenerationService _generationService;
    public OllamaTextEmbeddingGeneration(IConfiguration configuration)
    {
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOllamaTextEmbeddingGeneration(
            modelId: configuration["Ollama:ModelName"],           // E.g. "mxbai-embed-large" if mxbai-embed-large was downloaded as described above.
            endpoint: new Uri(configuration["Ollama:BaseUrl"]) // E.g. "http://localhost:11434" if Ollama has been started in docker as described above.
            //serviceId: "SERVICE_ID"             // Optional; for targeting specific services within Semantic Kernel
        );
        Kernel kernel = kernelBuilder.Build();
        _generationService = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
    }
    
    public async Task<ReadOnlyMemory<float>> GenerateTextEmbedding(string searchValue)
    {
        return await _generationService.GenerateEmbeddingAsync(searchValue);
    }
}