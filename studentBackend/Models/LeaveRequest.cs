using System;

namespace StudentAttendance.Models
{
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class LeaveRequest
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}