using OpenQA.Selenium;
using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface IScraperService
    {
        Task<WebScrapeResult> ScrapeWebsiteAsync(
            string url,
            Dictionary<string, string> cssSelectors,
            List<By> cookieBys,
            bool includePagination = false,
            string paginationSelector = null,
            int maxPages = 1);
    }
}
