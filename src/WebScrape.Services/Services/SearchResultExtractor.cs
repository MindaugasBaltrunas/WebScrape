using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;

namespace WebScrape.Services.Services
{
    public class SearchResultExtractor : ISearchResultExtractor
    {
        private readonly ILogger<SearchResultExtractor> _logger;

        public SearchResultExtractor(ILogger<SearchResultExtractor> logger)
        {
            _logger = logger;
        }

        public SearchResult ExtractResult(IWebElement container, string keyword)
        {
            try
            {
                var titleElement = container.FindElement(By.XPath(".//h3"));
                if (titleElement == null || string.IsNullOrWhiteSpace(titleElement.Text))
                {
                    _logger.LogDebug("No title found in result container");
                    return null;
                }

                var linkElement = container.FindElements(By.TagName("a"))
                    .FirstOrDefault(a =>
                        !string.IsNullOrEmpty(a.GetAttribute("href")) &&
                        a.GetAttribute("href").StartsWith("http") &&
                        !a.GetAttribute("href").Contains("google.com/search"));
                if (linkElement == null)
                {
                    _logger.LogDebug("No valid link found in result container");
                    return null;
                }

                string snippet = string.Empty;
                var divs = container.FindElements(By.TagName("div"));
                foreach (var div in divs)
                {
                    var text = div.Text;
                    if (!string.IsNullOrWhiteSpace(text) && text.Length > 50 && text.Length < 500 && text.Contains(" "))
                    {
                        snippet = text;
                        break;
                    }
                }

                return new SearchResult
                {
                    Keyword = keyword,
                    Title = titleElement.Text,
                    Url = linkElement.GetAttribute("href"),
                    Snippet = snippet,
                    ScrapedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract data from container");
                return null;
            }
        }
    }
}