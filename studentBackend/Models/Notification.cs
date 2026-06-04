using System;

namespace StudentAttendance.Models
{
    public enum NotificationType
    {
        Absent,
        AttendanceWarning,
        ParentInvite,
        LeaveApproved,
        LeaveRejected
    }

    public class Notification
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public string RecipientEmail { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}