using Microsoft.AspNetCore.Mvc;
using WebScrape.Core.Interfaces;
using WebScrape.Core.Models;

[ApiController]
[Route("api/[controller]")]
public class SearchJobsController : ControllerBase
{
    private readonly ISearchJobRepository _jobRepository;
    private readonly IGoogleScraperService _scraperService;

    public SearchJobsController(
        IGoogleScraperService scraperService,
        ISearchJobRepository jobRepository,
        ILogger<SearchJobsController> logger)
    {
        _jobRepository = jobRepository;
        _scraperService = scraperService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchJob>>> GetAllJobs()
        => Ok(await _jobRepository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<SearchJob>> GetJobById(int id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        return job is not null ? Ok(job) : NotFound();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SearchResult>>> GetByKeyword(
       [FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest("Keyword is required");

        var results = await _jobRepository.GetByKeywordAsync(keyword);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<SearchResult>>> CreateJob(
        [FromBody] CreateJobRequest request)
    {
        var results = await _scraperService.ScrapeGoogleAsync(request.Keyword, request.MaxResults);
        return Ok(results);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobById(int id)
    {
        await _jobRepository.DeleteAsync(id);
        return NoContent();
    }
}
