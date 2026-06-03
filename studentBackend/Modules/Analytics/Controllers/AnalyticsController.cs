using Microsoft.AspNetCore.Mvc;
using StudentAttendance.Modules.Analytics.Services;

namespace StudentAttendance.Modules.Analytics.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _service;

    public AnalyticsController(IAnalyticsService service)
    {
        _service = service;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var data = await _service.GetDashboardStatsAsync();
        return Ok(data);
    }

    [HttpGet("risk")]
    public async Task<IActionResult> GetRiskAnalysis()
    {
        var data = await _service.GetRiskAnalysisAsync();
        return Ok(data);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetAttendanceSummary()
    {
        var data = await _service.GetAttendanceSummaryAsync();
        return Ok(data);
    }

    [HttpGet("trend")]
    public async Task<IActionResult> GetAttendanceTrend([FromQuery] int months = 6)
    {
        var data = await _service.GetAttendanceTrendAsync(months);
        return Ok(data);
    }
}