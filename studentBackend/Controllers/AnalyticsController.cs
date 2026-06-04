using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("dashboard-stats")]
        [Authorize(Policy = "AllRoles")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _analyticsService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet("risk-analysis")]
        [Authorize(Policy = "TeacherOnly")]
        public async Task<IActionResult> GetRiskAnalysis()
        {
            var analysis = await _analyticsService.GetRiskAnalysisAsync();
            return Ok(analysis);
        }
    }
}