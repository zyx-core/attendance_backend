namespace StudentAttendance.DTOs
{
    public class AttendancePercentageDto
    {
        public int StudentId { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public double AttendancePercentage { get; set; }
    }

    public class ClassAverageAttendanceDto
    {
        public string? ClassName { get; set; }
        public int TotalStudents { get; set; }
        public double AverageAttendancePercentage { get; set; }
    }

    public class DailyAttendanceReportDto
    {
        public DateTime Date { get; set; }
        public int TotalStudents { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
    }

    public class MonthlyAttendanceReportDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalWorkingDays { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AverageAttendancePercentage { get; set; }
    }
}
