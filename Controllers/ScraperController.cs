using WebScrape.Core.Models;
using Microsoft.AspNetCore.Mvc;
using WebScrape.Core.Interfaces;

namespace WebScrape.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperFactory _scraperFactory;
        private readonly ISelectorMapper _selectorMapper;

        public ScraperController(IScraperFactory scraperFactory, ISelectorMapper selectorMapper)
        {
            _scraperFactory = scraperFactory;
            _selectorMapper = selectorMapper;
        }

        [HttpPost("scrapeBasic")]
        public ActionResult<Dictionary<string, List<string>>> ScrapeBasic([FromBody] BasicScrapeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required");

            if (request.ClassSelectors == null || request.ClassSelectors.Count == 0)
                return BadRequest("Class selectors are required");

            var cookieBys = _selectorMapper.MapToBy(request.CookieSelectors);
            var results = _scraperFactory.ScrapeWebsite(
                               request.Url,
                               request.ClassSelectors,
                               cookieBys
                           );

            return Ok(results);
        }

        [HttpPost("scrapePaginated")]
        public ActionResult<Dictionary<string, List<string>>> ScrapePaginated([FromBody] PaginatedScrapeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required");

            if (request.ClassSelectors == null || request.ClassSelectors.Count == 0)
                return BadRequest("Class selectors are required");

            if (string.IsNullOrWhiteSpace(request.PaginationSelector))
                return BadRequest("Pagination selector is required");

            var cookieBys = _selectorMapper.MapToBy(request.CookieSelectors);
            var results = _scraperFactory.ScrapeWebsiteWithPagination(
                                request.Url,
                                request.ClassSelectors,
                                request.PaginationSelector,
                                request.MaxPages ?? 5,
                                cookieBys
                            );

            return Ok(results);
        }

        [HttpPost("scrapeWithSelectors")]
        public ActionResult<Dictionary<string, List<string>>> ScrapeWithSelectors([FromBody] SelectorScrapeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required");

            if (request.Selectors == null || request.Selectors.Count == 0)
                return BadRequest("Selectors are required");

            var domainSelectors = _selectorMapper.MapToDomainSelectors(request.Selectors);
            var cookieBys = _selectorMapper.MapToBy(request.CookieSelectors);

            var results = _scraperFactory.ScrapeBySelectors(
                              request.Url,
                              domainSelectors,
                              cookieBys
                          );

            return Ok(results);
        }
    }
}
