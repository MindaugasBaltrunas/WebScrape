using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebScrape.Api.MapperProfile;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Repositories;
using WebScrape.Infrastructure.Data.Context;
using WebScrape.Services.Services;

namespace WebScrape.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1) Controllers + JSON options (case‐insensitive + string enums)
            services
                .AddControllers()
                .AddJsonOptions(opts =>
                {
                    var json = opts.JsonSerializerOptions;
                    json.PropertyNameCaseInsensitive = true;
                    json.Converters.Add(new JsonStringEnumConverter());
                });

            // 2) Swagger & enable [SwaggerRequestBody]
            services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "WebScrape API", Version = "v1" });
                });

            // 3) EF/Core
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("WebScrape.Infrastructure")
                ));

            // 4) Core services
            services.AddScoped<ICookieConsentHandler, CookieConsentHandler>();
            services.AddScoped<ISearchResultExtractor, SearchResultExtractor>();
            services.AddScoped<IGoogleScraperService, GoogleScraperService>();
            services.AddScoped<ISearchJobRepository, GoogleScraperRepository>();
            services.AddScoped<IScraperService, ScraperService>();

            // 5) Mapper
            services.AddSingleton<ISelectorMapper, SelectorMapper>();

            return services;
        }
    }
}