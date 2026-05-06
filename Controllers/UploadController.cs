using Microsoft.AspNetCore.Mvc;
using MediaPortal.Api.Models;
using MediaPortal.Api.Services;

namespace MediaPortal.Api.Controllers;

[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly DashboardService _dashboard;

    public UploadController(DashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    // POST /api/upload
    // Body: { "mediaBuy": [...], "conversions": [...] }
    [HttpPost]
    public async Task<IActionResult> Upload([FromBody] UploadRequest request)
    {
        if (request.MediaBuy.Count == 0 && request.Conversions.Count == 0)
            return BadRequest(new { error = "Request must contain at least one mediaBuy or conversions row." });

        try
        {
            var result = await _dashboard.UploadAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
