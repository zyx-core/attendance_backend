using System.Collections.Generic;

namespace StudentAttendance.DTOs
{
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
        public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High
    }

    public class RiskAnalysisDto
    {
        public int LowRiskCount { get; set; }
        public int MediumRiskCount { get; set; }
        public int HighRiskCount { get; set; }
        public List<AttendanceSummaryDto> AtRiskStudents { get; set; } = new();
    }
}