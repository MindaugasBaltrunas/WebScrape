
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;
using WebScrape.Infrastructure.Data.Context;

namespace WebScrape.Infrastructure.Repositories
{
    public class ScraperRepository : IScraperRepository
    {
        private readonly AppDbContext _context;

        public ScraperRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(WebScrapeResult result)
        {
            _context.WebScrapeResults.Add(result);
            await _context.SaveChangesAsync();
        }
    }
}

