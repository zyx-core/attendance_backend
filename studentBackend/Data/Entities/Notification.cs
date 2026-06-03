using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

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
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? StudentId { get; set; }

    [Required]
    [MaxLength(150)]
    public string RecipientEmail { get; set; } = string.Empty;

    [Required]
    public NotificationType Type { get; set; }

    [MaxLength(255)]
    public string? Subject { get; set; }

    public string? Body { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsSuccess { get; set; } = true;

    // Navigation properties
    [ForeignKey("StudentId")]
    public Student? Student { get; set; }
}
