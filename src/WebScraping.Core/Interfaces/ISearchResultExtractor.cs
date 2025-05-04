using OpenQA.Selenium;
using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface ISearchResultExtractor
    {
        SearchResult ExtractResult(IWebElement container, string keyword);
    }
}

