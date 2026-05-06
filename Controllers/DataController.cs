using Microsoft.AspNetCore.Mvc;
using MediaPortal.Api.Services;

namespace MediaPortal.Api.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly DashboardService _dashboard;
    private readonly ILogger<DataController> _logger;

    public DataController(DashboardService dashboard, ILogger<DataController> logger)
    {
        _dashboard = dashboard;
        _logger    = logger;
    }

    // GET /api/data?from=2026-04-01&to=2026-04-30&conversions=1
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? from,
        [FromQuery] string? to,
        [FromQuery] int conversions = 1)
    {
        var today       = DateTime.UtcNow;
        var defaultFrom = new DateOnly(today.Year, today.Month, 1);
        var defaultTo   = DateOnly.FromDateTime(today);

        DateOnly parsedFrom, parsedTo;

        if (from is not null)
        {
            if (!DateOnly.TryParseExact(from, "yyyy-MM-dd", out parsedFrom))
                return BadRequest(new { error = "Invalid 'from' date. Use YYYY-MM-DD." });
        }
        else
        {
            parsedFrom = defaultFrom;
        }

        if (to is not null)
        {
            if (!DateOnly.TryParseExact(to, "yyyy-MM-dd", out parsedTo))
                return BadRequest(new { error = "Invalid 'to' date. Use YYYY-MM-DD." });
        }
        else
        {
            parsedTo = defaultTo;
        }

        if (parsedFrom > parsedTo)
            return BadRequest(new { error = "'from' cannot be greater than 'to'." });

        try
        {
            var payload = await _dashboard.GetDashboardPayloadAsync(parsedFrom, parsedTo, conversions != 0);

            if (!payload.MediaBuy.Any())
                return NotFound(new { error = "No data found for the requested date range." });

            return Ok(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch dashboard payload for {From}–{To}", parsedFrom, parsedTo);
            return StatusCode(500, new { error = "An error occurred while fetching dashboard data." });
        }
    }
}
