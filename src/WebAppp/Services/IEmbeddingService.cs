
namespace WebAppp.Services
{
    public interface IEmbeddingService
    {
        Task<List<SearchResult>> SearchContent(string query);
        Task<List<SearchResult>> SearchEmbeddings(string query);
        Task StoreEmbeddings();
    }
}