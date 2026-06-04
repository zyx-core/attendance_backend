using Microsoft.AspNetCore.Http;
using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface IStudentService
    {
        Task<List<StudentResponseDto>> GetStudentsAsync();
        Task<StudentResponseDto?> GetStudentByIdAsync(int id);
        Task<List<StudentResponseDto>> SearchStudentsAsync(string? keyword, string? className, string? status);
        Task<StudentResponseDto> CreateStudentAsync(StudentCreateRequestDto dto);
        Task<StudentResponseDto?> UpdateStudentAsync(int id, StudentUpdateRequestDto dto);
        Task<bool> DeleteStudentAsync(int id);
        Task<StudentResponseDto?> UploadPhotoAsync(int id, IFormFile file, HttpRequest request);
    }
}
