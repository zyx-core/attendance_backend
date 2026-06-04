using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceResponseDto> MarkAttendanceAsync(
            CreateAttendanceDto dto);

        Task<AttendanceResponseDto?> UpdateAttendanceAsync(
            int id,
            CreateAttendanceDto dto);

        Task<IEnumerable<AttendanceResponseDto>>
            GetAttendanceHistoryAsync(int studentId);

        Task<AttendancePercentageDto> GetAttendancePercentageAsync(
            int studentId);

        Task<ClassAverageAttendanceDto> GetClassAttendanceAverageAsync(
            string? className);

        Task<DailyAttendanceReportDto> GetDailyReportAsync(
            DateTime date);

        Task<MonthlyAttendanceReportDto> GetMonthlyReportAsync(
            int month,
            int year);
    }
}
