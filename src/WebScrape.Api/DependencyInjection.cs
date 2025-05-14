using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebScrape.Api.MapperProfile;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Repositories;
using WebScrape.Infrastructure.Data.Context;
using WebScrape.Infrastructure.Repositories;
using WebScrape.Services.Services;

namespace WebScrape.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1) Controllers + JSON options (camelCase, null‐suppress, enums, ignore cycles)
            services
                .AddControllers()
                .AddJsonOptions(opts =>
                {
                    var json = opts.JsonSerializerOptions;

                    // Use camelCase for outgoing JSON properties
                    json.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    // Allow case-insensitive binding for incoming JSON
                    json.PropertyNameCaseInsensitive = true;
                    // Omit null-valued properties
                    json.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    // Prevent infinite loops by ignoring back-references
                    json.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    // Serialize enums as camelCase strings
                    json.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                    // (Optional) increase depth if truly required
                    // json.MaxDepth = 64;
                });

            // 2) Swagger & enable [SwaggerRequestBody]
            services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "WebScrape API", Version = "v1" });
                });

            // 3) EF Core / PostgreSQL
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("WebScrape.Infrastructure")
                ));

            // 4) Core services & repositories
            services.AddScoped<ICookieConsentHandler, CookieConsentHandler>();
            services.AddScoped<ISearchResultExtractor, SearchResultExtractor>();
            services.AddScoped<IGoogleScraperService, GoogleScraperService>();
            services.AddScoped<ISearchJobRepository, GoogleScraperRepository>();
            services.AddScoped<IScraperService, ScraperService>();
            services.AddScoped<IScraperRepository, ScraperRepository>();

            // 5) Mapper
            services.AddSingleton<ISelectorMapper, SelectorMapper>();

            return services;
        }
    }
}
