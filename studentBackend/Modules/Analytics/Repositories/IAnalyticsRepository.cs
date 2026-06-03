using StudentAttendance.Modules.Analytics.DTOs;

namespace StudentAttendance.Modules.Analytics.Repositories;

public interface IAnalyticsRepository
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<RiskAnalysisDto> GetRiskAnalysisAsync();
    Task<IEnumerable<AttendanceSummaryDto>> GetAttendanceSummaryAsync();
    Task<IEnumerable<AttendanceTrendDto>> GetAttendanceTrendAsync(int months);
}
