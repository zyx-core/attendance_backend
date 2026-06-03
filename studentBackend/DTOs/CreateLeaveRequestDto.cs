using System;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.DTOs
{
    public class CreateLeaveRequestDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}