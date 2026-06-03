namespace StudentAttendance.DTOs
{
    public class AttendanceResponseDto
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public DateTime Date { get; set; }

        public bool IsPresent { get; set; }

        public string? Remarks { get; set; }
    }
}