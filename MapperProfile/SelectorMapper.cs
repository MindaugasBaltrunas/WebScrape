using OpenQA.Selenium;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;
namespace WebScrape.Api.MapperProfile
{
    public class SelectorMapper : ISelectorMapper
    {
        public List<By> MapToBy(List<SelectorTypeRequest> requests)
        {
            if (requests == null)
                return new List<By>();
            return requests.Select(CreateByFromRequest).ToList();
        }
        private By CreateByFromRequest(SelectorTypeRequest dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            return dto.Type switch
            {
                SelectorTypes.ClassName => By.ClassName(dto.Value),
                SelectorTypes.CssSelector => By.CssSelector(dto.Value),
                SelectorTypes.XPath => By.XPath(dto.Value),
                SelectorTypes.Id => By.Id(dto.Value),
                SelectorTypes.TagName => By.TagName(dto.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(dto.Type), dto.Type, "Unknown selector type")
            };
        }
    }
}