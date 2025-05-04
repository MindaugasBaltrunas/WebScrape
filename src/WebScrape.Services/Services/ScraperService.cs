using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;

namespace WebScrape.Services.Services
{
    public class ScraperService : IScraperService
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly ICookieConsentHandler _cookieConsentHandler;
        private readonly ILogger<ScraperService> _logger;
        private readonly IScraperRepository _scraperRepository
;

        public ScraperService(
            ILogger<ScraperService> logger,
            ICookieConsentHandler cookieConsentHandler,
            IScraperRepository scraperRepository,
            bool headless = true)
        {
            _cookieConsentHandler = cookieConsentHandler ?? throw new ArgumentNullException(nameof(cookieConsentHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scraperRepository = scraperRepository ?? throw new ArgumentNullException(nameof(scraperRepository));

            var options = new ChromeOptions();
            if (headless)
                options.AddArgument("--headless");

            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--start-maximized");
            options.AddArgument("--window-size=1920,1080");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            _wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException),
                typeof(ElementNotInteractableException),
                typeof(JavaScriptException)
            );
        }

        public async Task<WebScrapeResult> ScrapeWebsiteAsync(
            string url,
            Dictionary<string, string> cssSelectors,
            List<By> cookieBys,
            bool includePagination = false,
            string paginationSelector = null,
            int maxPages = 1)
        {
            var buffer = cssSelectors
                .Keys
                .ToDictionary(k => k, k => new List<string>());

            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    _logger.LogError("URL cannot be null or empty");
                    return new WebScrapeResult
                    {
                        Url = url,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                url = EnsureValidUrl(url);
                _logger.LogInformation($"Navigating to: {url}");

                NavigateToUrl(url);
                _cookieConsentHandler.TryAcceptCookies(_driver, cookieBys);
                WaitForProductTiles();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Scraping error: {ex.Message}");
            }

            var result = new WebScrapeResult
            {
                Url = url,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var key in buffer.Keys)
            {
                string value = string.Join(", ", buffer[key]);

                switch (key.ToLowerInvariant())
                {
                    case "title":
                        result.Title = value;
                        break;
                    case "location":
                        result.Content_1 = value;
                        break;
                    case "price":
                        result.Content_2 = value;
                        break;
                    case "content":
                        result.Content_3 = value;
                        break;
                    case "description":
                        result.Content_4 = value;
                        break;
                    default:
                        if (key.EndsWith("_1")) result.Content_1 = value;
                        else if (key.EndsWith("_2")) result.Content_2 = value;
                        else if (key.EndsWith("_3")) result.Content_3 = value;
                        else if (key.EndsWith("_4")) result.Content_4 = value;
                        break;
                }
            }

            try
            {
                await _scraperRepository.CreateAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving scrape result: {ex.Message}");
            }

            return result;
        }


        private void WaitForProductTiles()
        {
            const string productCss = "a[role='article']";
            try
            {
                _wait.Until(d => d.FindElements(By.CssSelector(productCss)).Any());
                _logger.LogDebug("Product tiles loaded");
            }
            catch (WebDriverTimeoutException)
            {
                _logger.LogWarning("Timeout waiting for product tiles; continuing");
            }
        }

        private void NavigateToUrl(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                _wait.Until(d => ((IJavaScriptExecutor)d)
                    .ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (WebDriverException ex)
            {
                _logger.LogError($"Navigation error: {ex.Message}");
                throw;
            }
        }

        private string EnsureValidUrl(string url)
        {
            url = url.Trim();
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;

            if (!Regex.IsMatch(url, @"^https?://[\w\-_]+(\.[\w\-_]+)+"))
                url = url.Replace(" ", "%20");

            return url;
        }
        public void Close() => _driver.Quit();
    }
}
