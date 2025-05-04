
namespace WebScrape.Core.Models
{
    public class BasicScrapeRequest
    {
        public string Url { get; set; }
        public Dictionary<string, string> CssSelectors { get; set; }
        public List<SelectorTypeRequest> CookieSelectors { get; set; }
    }
}

