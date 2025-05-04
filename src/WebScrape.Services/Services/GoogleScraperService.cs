using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;

namespace WebScrape.Services.Services
{
    public class GoogleScraperService : IGoogleScraperService
    {
        private readonly ILogger<GoogleScraperService> _logger;
        private readonly ISearchJobRepository _repo;
        private readonly ICookieConsentHandler _cookieHandler;
        private readonly ISearchResultExtractor _extractor;

        public GoogleScraperService(
            ILogger<GoogleScraperService> logger,
            ISearchJobRepository repo,
            ICookieConsentHandler cookieHandler,
            ISearchResultExtractor extractor)
        {
            _logger = logger;
            _repo = repo;
            _cookieHandler = cookieHandler;
            _extractor = extractor;
        }

        public async Task<List<SearchResult>> ScrapeGoogleAsync(string keyword, int maxResults = 10)
        {
            _logger.LogInformation("Starting scrape for '{Keyword}'", keyword);
            var results = new List<SearchResult>();

            try
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set up ChromeDriver");
                return results;
            }

            var options = new ChromeOptions();
            options.AddArguments(
                "--headless",
                "--disable-gpu",
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--disable-blink-features=AutomationControlled",
                "--window-size=1920,1080"
            );
            var uas = new[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/118.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 Chrome/119.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0"
            };
            options.AddArgument($"--user-agent={uas[new Random().Next(uas.Length)]}");

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            using var driver = new ChromeDriver(service, options);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            try
            {
                var searchUrl = $"https://www.google.com/search?q={Uri.EscapeDataString(keyword)}&hl=en&num=100";
                _logger.LogDebug("Navigating to search URL: {Url}", searchUrl);
                driver.Navigate().GoToUrl(searchUrl);

                _cookieHandler.TryAcceptCookies(driver);
                await Task.Delay(2000);

                var containers = driver.FindElements(By.CssSelector("div.MjjYud")).ToList();
                if (!containers.Any())
                {
                    _logger.LogWarning("No search result containers found");
                    return results;
                }
                _logger.LogInformation("Found {Count} result containers", containers.Count);

                int count = 0;
                foreach (var container in containers)
                {
                    if (count >= maxResults) break;
                    try
                    {
                        var result = _extractor.ExtractResult(container, keyword);
                        if (result != null)
                        {
                            results.Add(result);
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing container");
                    }
                }

                var currentResult = new SearchJob();
                currentResult.Keyword = keyword;
                currentResult.CompletedAt = DateTime.UtcNow;

                foreach (var item in results)
                {
                    try
                    {
                        currentResult.Results.Add(item);
                        _logger.LogDebug("Saved result: {Title}", item.Title);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save '{Title}'", item.Title);
                    }
                }

                await _repo.CreateAsync(currentResult);

                _logger.LogInformation("Scrape complete: {Count} results saved for '{Keyword}'", results.Count, keyword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scraping");
            }

            return results;
        }
    }
}