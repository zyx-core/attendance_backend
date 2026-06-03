namespace StudentAttendance.Modules.Dashboard.DTOs;

public class TeacherDashboardDto
{
    public int TotalStudents { get; set; }
    public decimal AttendanceAverage { get; set; }
    public int PendingLeaves { get; set; }
    public int RiskStudents { get; set; }
}

public class ParentDashboardDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal CurrentAttendance { get; set; }
    public int ApprovedLeaves { get; set; }
    public int PendingLeaves { get; set; }
}
