using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceResponseDto> MarkAttendanceAsync(CreateAttendanceDto dto);

        Task<IEnumerable<AttendanceResponseDto>>
            GetAttendanceHistoryAsync(int studentId);

        Task<double> GetAttendancePercentageAsync(int studentId);
    }
}