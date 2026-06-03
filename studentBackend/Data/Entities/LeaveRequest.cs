using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected
}

public class LeaveRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int ParentId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    public int? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;

    [ForeignKey("ParentId")]
    public User Parent { get; set; } = null!;

    [ForeignKey("ReviewedBy")]
    public User? Reviewer { get; set; }
}
