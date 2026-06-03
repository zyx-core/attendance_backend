using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public enum UserRole
{
    Admin,
    Teacher,
    Parent
}

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Student> CreatedStudents { get; set; } = new List<Student>();
    public ICollection<ParentStudent> ParentStudents { get; set; } = new List<ParentStudent>();
    public ICollection<Attendance> MarkedAttendances { get; set; } = new List<Attendance>();
    public ICollection<LeaveRequest> LeaveRequestsAsParent { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveRequest> ReviewedLeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<ParentInvitation> CreatedInvitations { get; set; } = new List<ParentInvitation>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
