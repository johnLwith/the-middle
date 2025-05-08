using System.Threading.Tasks;

namespace WebAppp.Services
{
    public interface ITranslateService
    {
        Task<string> TranslateWordAsync(string word);
    }
}