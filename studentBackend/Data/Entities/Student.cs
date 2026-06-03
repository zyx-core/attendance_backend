using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RollNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? LastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string ClassName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Division { get; set; }

    [MaxLength(150)]
    public string? StudentEmail { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;

    public ICollection<ParentStudent> ParentStudents { get; set; } = new List<ParentStudent>();
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<ParentInvitation> Invitations { get; set; } = new List<ParentInvitation>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public AttendanceSummary? AttendanceSummary { get; set; }
}
