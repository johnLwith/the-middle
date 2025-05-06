using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using WebAppp.Data;
using WebAppp.Models;
using Microsoft.SemanticKernel.Connectors.Postgres;
using Npgsql;
using Microsoft.Extensions.VectorData;

namespace WebAppp.Services;

public class EmbeddingService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly OllamaTextEmbeddingGeneration _ollamaTextEmbeddingGeneration;
    
    private readonly PostgresVectorStoreRecordCollection<int, EpisodeEmbedding> _collection;

    public EmbeddingService(
        ApplicationDbContext context,
        IConfiguration configuration,
        HttpClient httpClient,
        OllamaTextEmbeddingGeneration ollamaTextEmbeddingGeneration)
    {
        _context = context;
        _httpClient = httpClient;
        _ollamaTextEmbeddingGeneration = ollamaTextEmbeddingGeneration;
        
        NpgsqlDataSourceBuilder dataSourceBuilder = new(configuration.GetConnectionString("DefaultConnection"));
        dataSourceBuilder.UseVector();
        var dataSource = dataSourceBuilder.Build();
        
        _collection = new PostgresVectorStoreRecordCollection<int, EpisodeEmbedding>(dataSource, "episodes_embeddings");
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

            if (string.IsNullOrEmpty(subtitleContent))
                continue;

            var entries = SrtParser.ParseSrtFile(subtitleContent);
            int i = 0;
            foreach (var entry in entries)
            {
                var currentContent = string.Join("", entry.Lines);
                // Generate embedding for the subtitle
                var embedding = await _ollamaTextEmbeddingGeneration.GenerateTextEmbedding(currentContent);

                // 3. Store the embedding in episodes_embeddings table
                var episodeEmbedding = new EpisodeEmbedding
                {
                    Id = i++,
                    EpisodeId = episode.Id,
                    Content = currentContent,
                    Embedding = embedding
                };

                await _collection.UpsertAsync(episodeEmbedding);
            }
        }

    }

    public async Task<List<VectorSearchResult<EpisodeEmbedding>>> SearchEmbeddings(string query)
    {
        // Generate embedding for the search query
        var searchEmbedding = await  _ollamaTextEmbeddingGeneration.GenerateTextEmbedding(query);

        var searchResults = await _collection.VectorizedSearchAsync(searchEmbedding);

        List<VectorSearchResult<EpisodeEmbedding>> results = new ();
        await foreach (var result in searchResults.Results)
        {
            results.Add(result);
        }
        return results;
    }
}

public class SearchResult
{
    public string Id { get; set; }
    public string EpisodeId { get; set; }
    public string Content { get; set; }
    public string EpisodeTitle { get; set; }
    public float Distance { get; set; }
}