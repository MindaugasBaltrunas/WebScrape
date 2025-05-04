using OpenQA.Selenium;
using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface ICookieConsentHandler
    {
        void TryAcceptCookies(IWebDriver driver, IEnumerable<By> selectorDefs = null);
    }
}

