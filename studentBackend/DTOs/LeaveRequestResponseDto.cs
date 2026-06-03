using System;

namespace StudentAttendance.DTOs
{
    public class LeaveRequestResponseDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}