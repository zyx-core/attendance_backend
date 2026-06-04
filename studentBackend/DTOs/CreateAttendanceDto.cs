using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.DTOs
{
    public class CreateAttendanceDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        public string? Remarks { get; set; }
    }
}