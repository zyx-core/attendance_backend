using StudentAttendance.Modules.Analytics.DTOs;
using StudentAttendance.Modules.Analytics.Repositories;

namespace StudentAttendance.Modules.Analytics.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _repository;

    public AnalyticsService(IAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public Task<DashboardStatsDto> GetDashboardStatsAsync() => _repository.GetDashboardStatsAsync();

    public Task<RiskAnalysisDto> GetRiskAnalysisAsync() => _repository.GetRiskAnalysisAsync();

    public Task<IEnumerable<AttendanceSummaryDto>> GetAttendanceSummaryAsync() => _repository.GetAttendanceSummaryAsync();

    public Task<IEnumerable<AttendanceTrendDto>> GetAttendanceTrendAsync(int months) => _repository.GetAttendanceTrendAsync(months);
}