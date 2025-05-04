namespace WebScrape.Core.Models
{
    public class WebScrapeResult
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content_1 { get; set; }
        public string Content_2 { get; set; }
        public string Content_3 { get; set; }
        public string Content_4 { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
