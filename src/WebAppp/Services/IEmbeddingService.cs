
namespace WebAppp.Services
{
    public interface IEmbeddingService
    {
        Task<List<SearchResult>> SearchEmbeddings(string query);
        Task StoreEmbeddings();
    }
}