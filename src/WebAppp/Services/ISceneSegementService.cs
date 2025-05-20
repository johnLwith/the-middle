using WebApp.Models;

namespace WebApp.Services
{
    public interface ISceneSegementService
    {
        Task<List<SegementSubtitleModel>> SegementAsync(string episodeId);
    }
}