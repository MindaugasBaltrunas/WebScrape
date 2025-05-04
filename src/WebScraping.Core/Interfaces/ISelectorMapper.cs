using OpenQA.Selenium;
using WebScrape.Core.Models;

namespace WebScrape.Core.Interfaces
{
    public interface ISelectorMapper
    {
        List<By> MapToBy(List<SelectorTypeRequest> requests);
    }
}
