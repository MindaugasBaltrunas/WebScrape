using Microsoft.AspNetCore.Mvc;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;

namespace WebScrape.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperFactory _scraperFactory;

        public ScraperController(IScraperFactory scraperFactory)
        {
            _scraperFactory = scraperFactory;
        }
        [HttpPost("scrapeBasic")]
        public ActionResult<Dictionary<string, List<string>>> ScrapeBasic([FromBody] BasicScrapeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Url))
                {
                    return BadRequest("URL is required");
                }

                if (request.ClassSelectors == null || request.ClassSelectors.Count == 0)
                {
                    return BadRequest("Class selectors are required");
                }

                var results = _scraperFactory.ScrapeWebsite(request.Url, request.ClassSelectors);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during basic scraping: {ex.Message}");
            }
        }

        [HttpPost("scrapePaginated")]
        public ActionResult<Dictionary<string, List<string>>> ScrapePaginated([FromBody] PaginatedScrapeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Url))
                {
                    return BadRequest("URL is required");
                }

                if (request.ClassSelectors == null || request.ClassSelectors.Count == 0)
                {
                    return BadRequest("Class selectors are required");
                }

                if (string.IsNullOrEmpty(request.PaginationSelector))
                {
                    return BadRequest("Pagination selector is required");
                }

                var results = _scraperFactory.ScrapeWebsiteWithPagination(
                    request.Url,
                    request.ClassSelectors,
                    request.PaginationSelector,
                    request.MaxPages ?? 5);

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during paginated scraping: {ex.Message}");
            }
        }

        [HttpPost("scrapeWithSelectors")]
        public ActionResult<Dictionary<string, List<string>>> ScrapeWithSelectors([FromBody] SelectorScrapeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Url))
                {
                    return BadRequest("URL is required");
                }

                if (request.Selectors == null || request.Selectors.Count == 0)
                {
                    return BadRequest("Selectors are required");
                }

                var selectorTypes = new Dictionary<string, SelectorType>();
                foreach (var selector in request.Selectors)
                {
                    selectorTypes[selector.Key] = new SelectorType(selector.Value.Value, selector.Value.Type);
                }

                var results = _scraperFactory.ScrapeBySelectors(request.Url, selectorTypes);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during selector-based scraping: {ex.Message}");
            }
        }
    }
}
