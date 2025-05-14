using Microsoft.EntityFrameworkCore;
using WebScrape.Core.Models;

namespace WebScrape.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<SearchJob> SearchJobs { get; set; }
        public DbSet<SearchResult> SearchResults { get; set; }
        public DbSet<WebScrapeResult> WebScrapeResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchJob>()
                .HasMany(j => j.Results)
                .WithOne(r => r.SearchJob)
                .HasForeignKey("SearchJobId");

            modelBuilder.Entity<SearchResult>()
                .HasIndex(r => r.Keyword);
        }
    }
}