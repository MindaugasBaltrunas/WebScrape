
namespace WebScrape.Core.Models
{
    public class SearchJob
    {
        public int Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public List<SearchResult> Results { get; set; } = new();
    }
}
