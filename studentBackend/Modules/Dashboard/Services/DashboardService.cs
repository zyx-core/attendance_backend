using StudentAttendance.Modules.Dashboard.DTOs;
using StudentAttendance.Modules.Analytics.Repositories;
using StudentAttendance.Modules.LeaveManagement.Repositories;

namespace StudentAttendance.Modules.Dashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly IAnalyticsRepository _analyticsRepository;
    private readonly ILeaveRepository _leaveRepository;

    public DashboardService(IAnalyticsRepository analyticsRepository, ILeaveRepository leaveRepository)
    {
        _analyticsRepository = analyticsRepository;
        _leaveRepository = leaveRepository;
    }

    public async Task<TeacherDashboardDto> GetTeacherDashboardAsync()
    {
        var stats = await _analyticsRepository.GetDashboardStatsAsync();
        return new TeacherDashboardDto
        {
            TotalStudents = stats.TotalStudents,
            AttendanceAverage = stats.AttendanceAverage,
            PendingLeaves = stats.PendingLeaves,
            RiskStudents = stats.RiskStudentsCount
        };
    }

    public async Task<IEnumerable<ParentDashboardDto>> GetParentDashboardAsync(int parentId)
    {
        // Placeholder for parent dashboard logic fetching their children and stats
        return new List<ParentDashboardDto>();
    }
}
