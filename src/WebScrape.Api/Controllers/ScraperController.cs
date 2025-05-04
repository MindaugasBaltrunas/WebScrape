using WebScrape.Core.Models;
using Microsoft.AspNetCore.Mvc;
using WebScrape.Core.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace WebScrape.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScraperController : ControllerBase
    {
        private readonly IScraperService _scraperService;
        private readonly ISelectorMapper _selectorMapper;
        private readonly ILogger<ScraperController> _logger;

        public ScraperController(
            IScraperService scraperService,
            ISelectorMapper selectorMapper,
            ILogger<ScraperController> logger)
        {
            _scraperService = scraperService ?? throw new ArgumentNullException(nameof(scraperService));
            _selectorMapper = selectorMapper ?? throw new ArgumentNullException(nameof(selectorMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("scrapeBasic")]
        public async Task<ActionResult<WebScrapeResult>> ScrapeBasic([FromBody] BasicScrapeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required");

            try
            {
                var cookieBys = _selectorMapper.MapToBy(request.CookieSelectors);

                var result = await _scraperService.ScrapeWebsiteAsync(
                    request.Url,
                    request.CssSelectors,
                    cookieBys
                );

                if (result == null)
                    return NotFound("Failed to retrieve any data from the website");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during web scraping");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}