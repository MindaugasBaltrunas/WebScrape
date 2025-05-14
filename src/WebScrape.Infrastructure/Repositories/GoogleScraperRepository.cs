using Microsoft.EntityFrameworkCore;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;
using WebScrape.Infrastructure.Data.Context;

namespace WebScrape.Core.Repositories
{
    public class GoogleScraperRepository : ISearchJobRepository
    {
        private readonly AppDbContext _context;

        public GoogleScraperRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SearchJob> GetByIdAsync(int id)
        {
            return await _context.SearchJobs
                .Include(j => j.Results)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<List<SearchJob>> GetAllAsync()
        {
            return await _context.SearchJobs
                .OrderByDescending(j => j.CompletedAt).ToListAsync();
        }

        public async Task CreateAsync(SearchJob result)
        {
            _context.SearchJobs.Add(result);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var job = await _context.SearchJobs.FindAsync(id);
            if (job != null)
            {
                _context.SearchJobs.Remove(job);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SearchJob> GetByKeywordAsync(string keyword)
        {
            return await _context.SearchJobs
                .Include(j => j.Results)
                .FirstOrDefaultAsync(j => j.Keyword == keyword);
        }
    }
}

