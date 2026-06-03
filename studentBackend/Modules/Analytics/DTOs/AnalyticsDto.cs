namespace StudentAttendance.Modules.Analytics.DTOs;

public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public decimal AttendanceAverage { get; set; }
    public int PendingLeaves { get; set; }
    public int RiskStudentsCount { get; set; }
}

public class AttendanceSummaryDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public decimal Percentage { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
}

public class RiskAnalysisDto
{
    public int LowRiskCount { get; set; }
    public int MediumRiskCount { get; set; }
    public int HighRiskCount { get; set; }
    public List<AttendanceSummaryDto> Students { get; set; } = new();
}

public class AttendanceTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal AveragePercentage { get; set; }
}