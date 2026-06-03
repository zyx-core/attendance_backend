using StudentAttendance.Modules.Analytics.DTOs;
using StudentAttendance.Data;
using Microsoft.EntityFrameworkCore;

namespace StudentAttendance.Modules.Analytics.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly AppDbContext _context;

    public AnalyticsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalStudents = await _context.Students.CountAsync();
        var pendingLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == Data.Entities.LeaveStatus.Pending);
        var highRisk = await _context.AttendanceSummaries.CountAsync(a => a.RiskLevel == Data.Entities.RiskLevel.High);
        
        var avg = await _context.AttendanceSummaries.AverageAsync(a => (decimal?)a.AttendancePercentage) ?? 0;

        return new DashboardStatsDto
        {
            TotalStudents = totalStudents,
            AttendanceAverage = avg,
            PendingLeaves = pendingLeaves,
            RiskStudentsCount = highRisk
        };
    }

    public async Task<RiskAnalysisDto> GetRiskAnalysisAsync()
    {
        var summaries = await _context.AttendanceSummaries.Include(a => a.Student).ToListAsync();
        
        var low = summaries.Count(s => s.RiskLevel == Data.Entities.RiskLevel.Low);
        var med = summaries.Count(s => s.RiskLevel == Data.Entities.RiskLevel.Medium);
        var high = summaries.Count(s => s.RiskLevel == Data.Entities.RiskLevel.High);

        var students = summaries.Select(s => new AttendanceSummaryDto
        {
            StudentId = s.StudentId,
            StudentName = s.Student != null ? $"{s.Student.FirstName} {s.Student.LastName}" : "Unknown",
            TotalClasses = s.TotalClasses,
            PresentCount = s.PresentCount,
            AbsentCount = s.AbsentCount,
            Percentage = s.AttendancePercentage,
            RiskLevel = s.RiskLevel.ToString()
        }).ToList();

        return new RiskAnalysisDto
        {
            LowRiskCount = low,
            MediumRiskCount = med,
            HighRiskCount = high,
            Students = students
        };
    }

    public async Task<IEnumerable<AttendanceSummaryDto>> GetAttendanceSummaryAsync()
    {
        var summaries = await _context.AttendanceSummaries.Include(a => a.Student).ToListAsync();
        return summaries.Select(s => new AttendanceSummaryDto
        {
            StudentId = s.StudentId,
            StudentName = s.Student != null ? $"{s.Student.FirstName} {s.Student.LastName}" : "Unknown",
            TotalClasses = s.TotalClasses,
            PresentCount = s.PresentCount,
            AbsentCount = s.AbsentCount,
            Percentage = s.AttendancePercentage,
            RiskLevel = s.RiskLevel.ToString()
        });
    }

    public Task<IEnumerable<AttendanceTrendDto>> GetAttendanceTrendAsync(int months)
    {
        // Placeholder for complex group by query
        return Task.FromResult<IEnumerable<AttendanceTrendDto>>(new List<AttendanceTrendDto>());
    }
}
