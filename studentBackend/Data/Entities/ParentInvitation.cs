using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public class ParentInvitation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(150)]
    public string ParentEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string InviteToken { get; set; } = string.Empty;

    public bool IsUsed { get; set; } = false;

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;
}
