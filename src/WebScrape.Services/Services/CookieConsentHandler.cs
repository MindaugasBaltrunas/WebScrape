using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using WebScrape.Core.Interfaces;

namespace WebScrape.Services.Services
{
    public class CookieConsentHandler : ICookieConsentHandler
    {
        private readonly ILogger<CookieConsentHandler> _logger;
        private readonly TimeSpan _timeout;
        private readonly IEnumerable<By> _defaultSelectors;

        public CookieConsentHandler(ILogger<CookieConsentHandler> logger, TimeSpan? timeout = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timeout = timeout ?? TimeSpan.FromSeconds(5);
            _defaultSelectors = new List<By>
            {
                By.XPath("//button[contains(text(), 'Accept all') or contains(text(), 'I agree') or contains(text(), 'Agree') or contains(@aria-label, 'accept') or contains(@id, 'consent')]"),
                By.CssSelector("button[class*='accept']"),
                By.CssSelector("button[id*='consent']"),
                By.CssSelector(".cookie-banner button"),
                By.CssSelector(".cc-btn.cc-accept"),
                By.CssSelector(".cookie-accept"),
            };
        }

        public void TryAcceptCookies(IWebDriver driver, IEnumerable<By> selectors = null)
        {
            try
            {
                var effectiveSelectors = (selectors ?? Enumerable.Empty<By>())
                                          .Concat(_defaultSelectors);
                var wait = new WebDriverWait(driver, _timeout)
                {
                    PollingInterval = TimeSpan.FromMilliseconds(500)
                };
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementClickInterceptedException));

                foreach (var selector in effectiveSelectors)
                {
                    var elements = driver.FindElements(selector);
                    foreach (var element in elements)
                    {
                        if (element.Displayed && element.Enabled)
                        {
                            try
                            {
                                element.Click();
                                _logger.LogInformation("Clicked cookie consent button using selector: {Selector}", selector);
                                return;
                            }
                            catch (ElementClickInterceptedException)
                            {
                                try
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
                                    _logger.LogInformation("Clicked cookie consent button using JavaScript with selector: {Selector}", selector);
                                    return;
                                }
                                catch (Exception jsEx)
                                {
                                    _logger.LogDebug(jsEx, "Failed to click using JavaScript");
                                }
                            }
                        }
                    }
                }
                _logger.LogDebug("No cookie consent button found among provided or default selectors.");
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error handling cookie consent (often normal)");
            }
        }
    }
}