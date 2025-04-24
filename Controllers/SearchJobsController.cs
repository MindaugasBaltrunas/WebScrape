using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;

namespace WebScrape.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchJobsController : ControllerBase
    {
        private readonly ISearchJobRepository _jobRepository;
        private readonly IScraperService _scraperService;
        private readonly ISearchResultRepository _resultRepository;
        private readonly ILogger<SearchJobsController> _logger;

        public SearchJobsController(
            ISearchJobRepository jobRepository,
            IScraperService scraperService,
            ISearchResultRepository resultRepository,
            ILogger<SearchJobsController> logger)
        {
            _jobRepository = jobRepository;
            _scraperService = scraperService;
            _resultRepository = resultRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SearchJob>>> GetAllJobs()
        {
            var jobs = await _jobRepository.GetAllAsync();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SearchJob>> GetJobById(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<SearchResult>>> CreateJob([FromBody] CreateJobRequest request)
        {
            var results = await _scraperService.ScrapeGoogleAsync(request.Keyword, request.maxResults);

            // Return the results wrapped in an ActionResult with Ok()
            return Ok(results);
        }
    }
}