using System.Threading.Tasks;
using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface IAnalyticsService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<RiskAnalysisDto> GetRiskAnalysisAsync();
    }
}