using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceResponseDto> MarkAttendanceAsync(
            CreateAttendanceDto dto);

        Task<bool> UpdateAttendanceAsync(
            int id,
            CreateAttendanceDto dto);

        Task<IEnumerable<AttendanceResponseDto>>
            GetAttendanceHistoryAsync(int studentId);

        Task<double> GetAttendancePercentageAsync(
            int studentId);

        Task<double> GetClassAttendanceAverageAsync();

        Task<object> GetDailyReportAsync(
            DateTime date);

        Task<object> GetMonthlyReportAsync(
            int month,
            int year);
    }
}