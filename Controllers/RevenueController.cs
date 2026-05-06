using MediaPortal.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediaPortal.Api.Controllers;

[ApiController]
[Route("api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueRepository _revenueRepository;
    private readonly ILogger<RevenueController> _logger;

    public RevenueController(IRevenueRepository revenueRepository, ILogger<RevenueController> logger)
    {
        _revenueRepository = revenueRepository;
        _logger            = logger;
    }

    // GET /api/revenue/by-publisher?publisher=Google-CPL&startDate=2026-01-01&endDate=2026-05-05
    [HttpGet("by-publisher")]
    public async Task<IActionResult> GetRevenueByPublisher(
        [FromQuery] string publisher,
        [FromQuery] string startDate,
        [FromQuery] string endDate)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            return BadRequest(new { error = "'publisher' query param is required." });

        if (!DateOnly.TryParseExact(startDate, "yyyy-MM-dd", out var parsedStart))
            return BadRequest(new { error = "Invalid 'startDate'. Use YYYY-MM-DD." });

        if (!DateOnly.TryParseExact(endDate, "yyyy-MM-dd", out var parsedEnd))
            return BadRequest(new { error = "Invalid 'endDate'. Use YYYY-MM-DD." });

        if (parsedStart > parsedEnd)
            return BadRequest(new { error = "'startDate' cannot be greater than 'endDate'." });

        try
        {
            var result = await _revenueRepository.GetRevenueByPublisherAsync(publisher, parsedStart, parsedEnd);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch revenue for publisher {Publisher} ({Start}–{End})",
                publisher, parsedStart, parsedEnd);
            return StatusCode(500, new { error = "An error occurred while fetching revenue data." });
        }
    }
}
