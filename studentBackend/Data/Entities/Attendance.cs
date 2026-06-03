using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public enum AttendanceStatus
{
    Present,
    Absent
}

public class Attendance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public DateTime AttendanceDate { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; }

    [Required]
    public int MarkedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;

    [ForeignKey("MarkedBy")]
    public User MarkedByUser { get; set; } = null!;
}
