using System;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendance.Models
{
    public class ParentInvitation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StudentId { get; set; }
        public string ParentEmail { get; set; } = string.Empty;
        public string InviteToken { get; set; } = string.Empty;
        public bool IsUsed { get; set; } = false;
        public DateTime ExpiresAt { get; set; }
    }
}
