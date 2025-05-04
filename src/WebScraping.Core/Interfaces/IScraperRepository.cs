using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface IScraperRepository
    {
        Task CreateAsync(WebScrapeResult result);
    }
}
