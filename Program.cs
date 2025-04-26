using Microsoft.EntityFrameworkCore;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Repositories;
using WebScrape.Infrastructure.Data.Context;
using WebScrape.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "WebScrape API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("WebScrape.Infrastructure")
    ));

builder.Services.AddScoped<IGoogleScraperService, GoogleScraperService>();
builder.Services.AddScoped<ISearchJobRepository, GoogleScraperRepository>();
builder.Services.AddScoped<ICookieConsentHandler, CookieConsentHandler>();
builder.Services.AddScoped<ISearchResultExtractor, SearchResultExtractor>();

builder.Services.AddScoped<IScraperFactory, ScraperFactory>();
builder.Services.AddScoped<IGoogleScraperService, ScraperService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebScrape API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
