using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendance.Data.Entities;

public class ParentStudent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ParentId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [MaxLength(50)]
    public string? Relationship { get; set; }

    // Navigation properties
    [ForeignKey("ParentId")]
    public User Parent { get; set; } = null!;

    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;
}
