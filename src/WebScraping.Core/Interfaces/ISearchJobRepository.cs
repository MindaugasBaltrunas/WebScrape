using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface ISearchJobRepository
    {
        Task<SearchJob> GetByIdAsync(int id);
        Task<List<SearchJob>> GetAllAsync();
        Task CreateAsync(SearchJob job);
        Task DeleteAsync(int id);
        Task<SearchJob> GetByKeywordAsync(string keyword);
    }
}
