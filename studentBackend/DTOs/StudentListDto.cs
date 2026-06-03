namespace studentBackend.DTOs;

public class StudentListDto
{
    public int Id { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
}
