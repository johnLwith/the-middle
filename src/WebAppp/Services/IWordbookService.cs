using System.Threading.Tasks;
using WebAppp.Models;

namespace WebAppp.Services
{
    public interface IWordbookService
    {
        Task<Word> AddWordAsync(string word, string episodeId);
        Task<IEnumerable<Word>> GetWordsByEpisodeIdAsync(string episodeId);
    }
}