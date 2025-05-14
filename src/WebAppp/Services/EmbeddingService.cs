using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Postgres;
using Microsoft.SemanticKernel.Embeddings;
using Npgsql;
using System.Text.RegularExpressions;
using WebAppp.Data;
using WebAppp.Models;

namespace WebAppp.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private const int QuerySize = 50;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly Kernel _kernel;
        private readonly PostgresVectorStoreRecordCollection<string, EpisodeEmbedding> _collection;

        public EmbeddingService(
            ApplicationDbContext context,
            IConfiguration configuration,
            HttpClient httpClient,
            Kernel kernel)
        {
            _context = context;
            _httpClient = httpClient;
            _kernel = kernel;

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            NpgsqlDataSourceBuilder dataSourceBuilder = new(connectionString);
            dataSourceBuilder.UseVector();
            var dataSource = dataSourceBuilder.Build();
            _collection = new PostgresVectorStoreRecordCollection<string, EpisodeEmbedding>(dataSource, "episodes_embeddings");
        }

        public async Task StoreEmbeddings()
        {
            // 1. Get all episodes with subtitles
            var episodes = await _context.Episodes
                .Where(e => e.SubtitlePath != null)
                .ToListAsync();

            // 2. For each episode, generate embeddings
            foreach (var episode in episodes)
            {
                if (string.IsNullOrEmpty(episode.SubtitlePath))
                    continue;

                // Read subtitle content from file
                var subtitleContent = await _httpClient.GetStringAsync(episode.SubtitlePath);
                var items = SrtParser.ParseSrtFile(subtitleContent);
                foreach (var item in items)
                {
                    // Generate embedding for the subtitle
                    var embedding = await GenerateEmbedding(item.Lines?.Aggregate((x, y)=> x + Environment.NewLine + y));

                    // 3. Store the embedding in episodes_embeddings table
                    int prefix = int.Parse(Regex.Matches(episode.Id, @"-?\d+").First().Value);
                    var episodeEmbedding = new EpisodeEmbedding
                    {
                        Id = prefix + item.SequenceNumber,
                        EpisodeId = episode.Id,
                        Content = subtitleContent,
                        Embedding = embedding
                    };
                    await _collection.UpsertAsync(episodeEmbedding);
                }
            }
        }

        private async Task<ReadOnlyMemory<float>> GenerateEmbedding(string text)
        {
#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001
            var generationService = _kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
            return await generationService.GenerateEmbeddingAsync(text);
        }

        public async Task<List<SearchResult>> SearchEmbeddings(string query)
        {
            var result = new List<SearchResult>();
            var searchEmbedding = await GenerateEmbedding(query);
            var searchResults = await _collection.VectorizedSearchAsync(searchEmbedding, new Microsoft.Extensions.VectorData.VectorSearchOptions<EpisodeEmbedding> { 
                Top = QuerySize
            });
            await foreach (var searchResult in searchResults.Results)
            {
                result.Add(new SearchResult
                {
                    Content = searchResult.Record.Content,
                    EpisodeId = searchResult.Record.EpisodeId,
                    Score = searchResult.Score,
                });
            }
            var episodeIds = result.Select(y => y.EpisodeId).ToList();
            var idTitles = _context.Episodes.Where(x => episodeIds.Contains(x.Id))
                                .Select(x => new SearchResult { EpisodeId = x.Id, Title = x.Title });
            foreach (var item in result)
            { 
                item.Title = idTitles.Where(x=>x.EpisodeId == item.EpisodeId).First().Title;
            }

            return result;
        }

        public async Task<List<SearchResult>> SearchContent(string query)
        {
            var result = await _context.EpisodeEmbeddings
                .Include(x=>x.Episode)
                .Where(x=>x.Content.Contains(query))
                .Take(50)
                .Select(x=>new SearchResult {
                    Content = x.Content,
                    EpisodeId = x.EpisodeId,
                    Title = x.Episode.Title
                })
                .ToListAsync();
            return result;
        }
    }

    public class SearchResult
    {
        public string EpisodeId { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; }
        public double? Score { get; set; }
    }
}