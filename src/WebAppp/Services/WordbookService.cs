using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebAppp.Data;
using WebAppp.Models;

namespace WebAppp.Services
{
    public class WordbookService : IWordbookService
    {
        private readonly ApplicationDbContext _context;

        public WordbookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Word> AddWordAsync(string wordText, string episodeId)
        {
            var word = new Word
            {
                WordText = wordText,
                EpisodeId = episodeId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Words.Add(word);
            await _context.SaveChangesAsync();

            return word;
        }

        public async Task<IEnumerable<Word>> GetWordsByEpisodeIdAsync(string episodeId)
        {
            return await _context.Words
                .Where(w => w.EpisodeId == episodeId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }
    }
}