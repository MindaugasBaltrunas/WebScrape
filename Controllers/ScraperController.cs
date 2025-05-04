using WebScrape.Core.Models;
using Microsoft.AspNetCore.Mvc;
using WebScrape.Core.Interfaces;
namespace WebScrape.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperService _scraperService;
        private readonly ISelectorMapper _selectorMapper;
        public ScraperController(IScraperService scraperService, ISelectorMapper selectorMapper)
        {
            _scraperService = scraperService;
            _selectorMapper = selectorMapper;
        }
        [HttpPost("scrapeBasic")]
        public ActionResult<Dictionary<string, List<string>>> ScrapeBasic([FromBody] BasicScrapeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required");
            var cookieBys = _selectorMapper.MapToBy(request.CookieSelectors);
            var results = _scraperService.ScrapeWebsite(
                               request.Url,
                               request.CssSelectors,
                               cookieBys
                           );
            return Ok(results);
        }
    }
}