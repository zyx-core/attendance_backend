using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [MaxLength(255)]
    public string? Action { get; set; }

    [MaxLength(100)]
    public string? EntityName { get; set; }

    public int? EntityId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
