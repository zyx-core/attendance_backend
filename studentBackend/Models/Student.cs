namespace studentBackend.Models;

public class Student
{
    public int Id { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? PhotoUrl { get; set; }
    public int ParentId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
