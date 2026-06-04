using Microsoft.EntityFrameworkCore;
using StudentAttendance.DTOs;
using StudentAttendance.Models;

namespace StudentAttendance.Services
{
    public class StudentService : IStudentService
    {
        private static readonly string[] AllowedImageTypes = ["image/jpeg", "image/png"];
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public StudentService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<StudentResponseDto>> GetStudentsAsync()
        {
            return await _context.Students
                .OrderBy(student => student.RollNumber)
                .Select(student => MapToResponse(student))
                .ToListAsync();
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            return student == null ? null : MapToResponse(student);
        }

        public async Task<List<StudentResponseDto>> SearchStudentsAsync(string? keyword, string? className, string? status)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim().ToLower();
                query = query.Where(student =>
                    student.FirstName.ToLower().Contains(term) ||
                    student.LastName.ToLower().Contains(term) ||
                    student.RollNumber.ToLower().Contains(term) ||
                    student.ClassName.ToLower().Contains(term) ||
                    student.StudentEmail.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(className))
            {
                var classTerm = className.Trim().ToLower();
                query = query.Where(student => student.ClassName.ToLower().Contains(classTerm));
            }

            if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(student => student.IsActive);
            }
            else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(student => !student.IsActive);
            }

            return await query
                .OrderBy(student => student.RollNumber)
                .Select(student => MapToResponse(student))
                .ToListAsync();
        }

        public async Task<StudentResponseDto> CreateStudentAsync(StudentCreateRequestDto dto)
        {
            var student = new Student
            {
                RollNumber = dto.RollNumber.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                ClassName = dto.ClassName.Trim(),
                Division = dto.Division.Trim(),
                StudentEmail = dto.StudentEmail.Trim(),
                DateOfBirth = dto.DateOfBirth,
                PhotoUrl = dto.PhotoUrl,
                ParentId = dto.ParentId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return MapToResponse(student);
        }

        public async Task<StudentResponseDto?> UpdateStudentAsync(int id, StudentUpdateRequestDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return null;
            }

            student.RollNumber = dto.RollNumber.Trim();
            student.FirstName = dto.FirstName.Trim();
            student.LastName = dto.LastName.Trim();
            student.ClassName = dto.ClassName.Trim();
            student.Division = dto.Division.Trim();
            student.StudentEmail = dto.StudentEmail.Trim();
            student.DateOfBirth = dto.DateOfBirth;
            student.PhotoUrl = dto.PhotoUrl;
            student.ParentId = dto.ParentId;
            student.IsActive = dto.IsActive;
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(student);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<StudentResponseDto?> UploadPhotoAsync(int id, IFormFile file, HttpRequest request)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return null;
            }

            if (file.Length == 0 || !AllowedImageTypes.Contains(file.ContentType))
            {
                throw new InvalidOperationException("Only non-empty JPEG and PNG images are allowed.");
            }

            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsPath = Path.Combine(webRoot, "uploads", "students");
            Directory.CreateDirectory(uploadsPath);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            student.PhotoUrl = $"{request.Scheme}://{request.Host}/uploads/students/{fileName}";
            student.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToResponse(student);
        }

        private static StudentResponseDto MapToResponse(Student student)
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
                DateOfBirth = student.DateOfBirth,
                PhotoUrl = student.PhotoUrl,
                ParentId = student.ParentId,
                IsActive = student.IsActive,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            };
        }
    }
}
