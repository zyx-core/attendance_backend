using System.ComponentModel.DataAnnotations;
using StudentAttendance.Data.Entities;

namespace StudentAttendance.Modules.LeaveManagement.DTOs;

public class LeaveRequestDto
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public string Reason { get; set; } = string.Empty;
}

public class LeaveResponseDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class LeaveApprovalDto
{
    [Required]
    public LeaveStatus Status { get; set; }
}
