using Microsoft.EntityFrameworkCore;
using WebScrape.Core.Interfaces;
using WebScrape.Infrastructure.Data.Context;
using WebScrape.Services.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("GoogleKeywordScraper.Infrastructure")));

// Register services
//builder.Services.AddScoped<ISearchJobRepository, SearchJobRepository>();
//builder.Services.AddScoped<ISearchResultRepository, SearchResultRepository>();
builder.Services.AddScoped<IScraperService, GoogleScraperService>();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
     dbContext.Database.Migrate();
}

app.Run();