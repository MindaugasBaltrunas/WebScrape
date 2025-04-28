using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using WebScrape.Api.MapperProfile;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Repositories;
using WebScrape.Infrastructure.Data.Context;
using WebScrape.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers + JSON options (case‐insensitive + string enums)
builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        var json = opts.JsonSerializerOptions;
        json.PropertyNameCaseInsensitive = true;
        json.Converters.Add(new JsonStringEnumConverter());
    });

// 2) Swagger & enable [SwaggerRequestBody]
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "WebScrape API", Version = "v1" });
    });

// 3) EF/Core
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("WebScrape.Infrastructure")
    ));

// 4) Core services
builder.Services.AddScoped<ICookieConsentHandler, CookieConsentHandler>();
builder.Services.AddScoped<ISearchResultExtractor, SearchResultExtractor>();
builder.Services.AddScoped<IScraperFactory>(sp =>
    new ScraperFactory(
        sp.GetRequiredService<ICookieConsentHandler>(),
        sp.GetRequiredService<IScraperService>(),
        true 
    )
);
builder.Services.AddScoped<IGoogleScraperService, GoogleScraperService>();
builder.Services.AddScoped<ISearchJobRepository, GoogleScraperRepository>();
builder.Services.AddScoped<IScraperService, ScraperService>();

// 5) Mapper
builder.Services.AddSingleton<ISelectorMapper, SelectorMapper>();

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

// apply migrations at startup
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();

app.Run();
