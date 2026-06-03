using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.Modules.Dashboard.Services;

namespace StudentAttendance.Modules.Dashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("teacher")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> GetTeacherDashboard()
    {
        var result = await _dashboardService.GetTeacherDashboardAsync();
        return Ok(result);
    }

    [HttpGet("parent")]
    [Authorize(Policy = "ParentOnly")]
    public async Task<IActionResult> GetParentDashboard()
    {
        var parentId = 1; // Mock from claims
        var result = await _dashboardService.GetParentDashboardAsync(parentId);
        return Ok(result);
    }
}
