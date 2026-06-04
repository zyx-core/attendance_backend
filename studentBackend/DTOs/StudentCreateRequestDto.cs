using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.DTOs
{
    public class StudentCreateRequestDto
    {
        [Required]
        public string RollNumber { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string ClassName { get; set; } = string.Empty;

        [Required]
        public string Division { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string StudentEmail { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? PhotoUrl { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
