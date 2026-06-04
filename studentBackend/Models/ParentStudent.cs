using System;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.Models
{
    public class ParentStudent
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParentId { get; set; }
        public Guid StudentId { get; set; }
        public string Relationship { get; set; } = string.Empty;
    }
}
