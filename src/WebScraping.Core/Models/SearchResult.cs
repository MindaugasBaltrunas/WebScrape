
namespace WebScrape.Core.Models
{
    public class SearchResult
    {
        public int Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Snippet { get; set; } = string.Empty;
        public DateTime ScrapedAt { get; set; }
        public int SearchJobId { get; set; }
        public SearchJob SearchJob { get; set; } = null;
    }
}
