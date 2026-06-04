using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string RollNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Division { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string StudentEmail { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public string? PhotoUrl { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
