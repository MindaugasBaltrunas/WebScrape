
namespace WebScrape.Core.Models
{
    public enum SelectorTypes
    {
        ClassName,
        CssSelector,
        XPath,
        Id,
        TagName
    }

    public class SelectorType
    {
        public string Value { get; set; }
        public SelectorTypes Type { get; set; }

        public SelectorType(string value, SelectorTypes type)
        {
            Value = value;
            Type = type;
        }
    }
}
