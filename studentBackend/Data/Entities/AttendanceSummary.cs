using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public enum RiskLevel
{
    Low,
    Medium,
    High
}

public class AttendanceSummary
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    public int TotalClasses { get; set; } = 0;

    public int PresentCount { get; set; } = 0;

    public int AbsentCount { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal AttendancePercentage { get; set; }

    public RiskLevel RiskLevel { get; set; }

    public DateTime? LastUpdated { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;
}
