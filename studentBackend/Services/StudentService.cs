using Microsoft.EntityFrameworkCore;
using studentBackend.Data;
using studentBackend.DTOs;
using studentBackend.Models;

namespace studentBackend.Services;

public class StudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentListDto>> GetStudentsAsync()
    {
        var students = await _context.Students
            .AsNoTracking()
            .OrderBy(student => student.RollNumber)
            .ToListAsync();

        return students.Select(ToListDto).ToList();
    }

    public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(student => student.Id == id);

        return student is null ? null : ToResponseDto(student);
    }

    public async Task<StudentResponseDto> CreateStudentAsync(StudentCreateDto studentCreateDto)
    {
        var student = new Student
        {
            RollNumber = studentCreateDto.RollNumber.Trim(),
            FirstName = studentCreateDto.FirstName.Trim(),
            LastName = studentCreateDto.LastName.Trim(),
            ClassName = studentCreateDto.ClassName.Trim(),
            Division = studentCreateDto.Division.Trim(),
            StudentEmail = studentCreateDto.StudentEmail.Trim(),
            DateOfBirth = studentCreateDto.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            PhotoUrl = string.IsNullOrWhiteSpace(studentCreateDto.PhotoUrl) ? null : studentCreateDto.PhotoUrl.Trim(),
            ParentId = studentCreateDto.ParentId,
            IsActive = studentCreateDto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return ToResponseDto(student);
    }

    public async Task<StudentResponseDto?> UpdateStudentAsync(int id, StudentUpdateDto studentUpdateDto)
    {
        var student = await _context.Students.FirstOrDefaultAsync(student => student.Id == id);
        if (student is null)
        {
            return null;
        }

        student.RollNumber = studentUpdateDto.RollNumber.Trim();
        student.FirstName = studentUpdateDto.FirstName.Trim();
        student.LastName = studentUpdateDto.LastName.Trim();
        student.ClassName = studentUpdateDto.ClassName.Trim();
        student.Division = studentUpdateDto.Division.Trim();
        student.StudentEmail = studentUpdateDto.StudentEmail.Trim();
        student.DateOfBirth = studentUpdateDto.DateOfBirth.ToDateTime(TimeOnly.MinValue);
        student.PhotoUrl = string.IsNullOrWhiteSpace(studentUpdateDto.PhotoUrl) ? null : studentUpdateDto.PhotoUrl.Trim();
        student.ParentId = studentUpdateDto.ParentId;
        student.IsActive = studentUpdateDto.IsActive;
        student.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToResponseDto(student);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(student => student.Id == id);
        if (student is null)
        {
            return false;
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<StudentListDto>> SearchStudentsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetStudentsAsync();
        }

        keyword = keyword.Trim();

        var students = await _context.Students
            .AsNoTracking()
            .Where(student =>
                student.RollNumber.Contains(keyword) ||
                student.FirstName.Contains(keyword) ||
                student.LastName.Contains(keyword) ||
                student.ClassName.Contains(keyword) ||
                student.Division.Contains(keyword) ||
                student.StudentEmail.Contains(keyword))
            .OrderBy(student => student.RollNumber)
            .ToListAsync();

        return students.Select(ToListDto).ToList();
    }

    private static StudentListDto ToListDto(Student student)
    {
        return new StudentListDto
        {
            Id = student.Id,
            RollNumber = student.RollNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            ClassName = student.ClassName,
            Division = student.Division,
            StudentEmail = student.StudentEmail,
            PhotoUrl = student.PhotoUrl,
            IsActive = student.IsActive
        };
    }

    private static StudentResponseDto ToResponseDto(Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            RollNumber = student.RollNumber,
            FirstName = student.FirstName,
            LastName = student.LastName,
            ClassName = student.ClassName,
            Division = student.Division,
            StudentEmail = student.StudentEmail,
            DateOfBirth = DateOnly.FromDateTime(student.DateOfBirth),
            PhotoUrl = student.PhotoUrl,
            ParentId = student.ParentId ?? 0,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt,
            UpdatedAt = student.UpdatedAt
        };
    }
}
