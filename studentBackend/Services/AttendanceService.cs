using Microsoft.EntityFrameworkCore;
using StudentAttendance.DTOs;
using StudentAttendance.Models;

namespace StudentAttendance.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceResponseDto> MarkAttendanceAsync(
            CreateAttendanceDto dto)
        {
            var exists = await _context.Attendances
                .AnyAsync(a =>
                    a.StudentId == dto.StudentId &&
                    a.Date.Date == dto.Date.Date);

            if (exists)
            {
                throw new Exception(
                    "Attendance already marked for this date.");
            }

            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                Date = dto.Date,
                IsPresent = dto.IsPresent,
                Remarks = dto.Remarks
            };

            _context.Attendances.Add(attendance);

            await _context.SaveChangesAsync();

            return new AttendanceResponseDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                Date = attendance.Date,
                IsPresent = attendance.IsPresent,
                Remarks = attendance.Remarks
            };
        }

        public async Task<AttendanceResponseDto?> UpdateAttendanceAsync(
            int id,
            CreateAttendanceDto dto)
        {
            var attendance =
                await _context.Attendances.FindAsync(id);

            if (attendance == null)
                return null;

            attendance.StudentId = dto.StudentId;
            attendance.Date = dto.Date;
            attendance.IsPresent = dto.IsPresent;
            attendance.Remarks = dto.Remarks;

            await _context.SaveChangesAsync();

            return new AttendanceResponseDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                Date = attendance.Date,
                IsPresent = attendance.IsPresent,
                Remarks = attendance.Remarks
            };
        }

        public async Task<IEnumerable<AttendanceResponseDto>>
            GetAttendanceHistoryAsync(int studentId)
        {
            return await _context.Attendances
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .Select(a => new AttendanceResponseDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    Date = a.Date,
                    IsPresent = a.IsPresent,
                    Remarks = a.Remarks
                })
                .ToListAsync();
        }

        public async Task<AttendancePercentageDto>
            GetAttendancePercentageAsync(int studentId)
        {
            var totalDays = await _context.Attendances
                .CountAsync(a => a.StudentId == studentId);

            var presentDays = await _context.Attendances
                .CountAsync(a =>
                    a.StudentId == studentId &&
                    a.IsPresent);

            if (totalDays == 0)
            {
                return new AttendancePercentageDto
                {
                    StudentId = studentId,
                    TotalDays = 0,
                    PresentDays = 0,
                    AbsentDays = 0,
                    AttendancePercentage = 0
                };
            }

            return new AttendancePercentageDto
            {
                StudentId = studentId,
                TotalDays = totalDays,
                PresentDays = presentDays,
                AbsentDays = totalDays - presentDays,
                AttendancePercentage = Math.Round((double)presentDays / totalDays * 100, 2)
            };
        }

        public async Task<ClassAverageAttendanceDto>
            GetClassAttendanceAverageAsync(string? className)
        {
            var studentQuery = _context.Students.AsQueryable();
            if (!string.IsNullOrWhiteSpace(className))
            {
                var normalizedClass = className.Trim().ToLower();
                studentQuery = studentQuery.Where(student => student.ClassName.ToLower() == normalizedClass);
            }

            var studentIds = await studentQuery.Select(student => student.Id).ToListAsync();
            var totalStudents = studentIds.Count;
            var attendanceQuery = _context.Attendances.Where(attendance => studentIds.Contains(attendance.StudentId));

            var totalRecords = await attendanceQuery.CountAsync();

            if (totalRecords == 0)
            {
                return new ClassAverageAttendanceDto
                {
                    ClassName = className,
                    TotalStudents = totalStudents,
                    AverageAttendancePercentage = 0
                };
            }

            var presentRecords = await attendanceQuery.CountAsync(a => a.IsPresent);

            return new ClassAverageAttendanceDto
            {
                ClassName = className,
                TotalStudents = totalStudents,
                AverageAttendancePercentage = Math.Round((double)presentRecords / totalRecords * 100, 2)
            };
        }

        public async Task<DailyAttendanceReportDto>
            GetDailyReportAsync(DateTime date)
        {
            var records = await _context.Attendances
                .Where(a => a.Date.Date == date.Date)
                .ToListAsync();

            return new DailyAttendanceReportDto
            {
                Date = date,
                TotalStudents = records.Count,
                TotalPresent = records.Count(x => x.IsPresent),
                TotalAbsent = records.Count(x => !x.IsPresent)
            };
        }

        public async Task<MonthlyAttendanceReportDto>
            GetMonthlyReportAsync(
                int month,
                int year)
        {
            var records = await _context.Attendances
                .Where(a =>
                    a.Date.Month == month &&
                    a.Date.Year == year)
                .ToListAsync();

            var totalRecords = records.Count;
            var presentCount = records.Count(x => x.IsPresent);

            return new MonthlyAttendanceReportDto
            {
                Month = month,
                Year = year,
                TotalWorkingDays = totalRecords,
                PresentCount = presentCount,
                AbsentCount = records.Count(x => !x.IsPresent),
                AverageAttendancePercentage = totalRecords == 0
                    ? 0
                    : Math.Round((double)presentCount / totalRecords * 100, 2)
            };
        }
    }
}
