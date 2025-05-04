using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface IGoogleScraperService
    {
        Task<List<SearchResult>> ScrapeGoogleAsync(string keyword, int maxResults = 10);
    }
}
