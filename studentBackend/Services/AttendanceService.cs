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

        public async Task<bool> UpdateAttendanceAsync(
            int id,
            CreateAttendanceDto dto)
        {
            var attendance =
                await _context.Attendances.FindAsync(id);

            if (attendance == null)
                return false;

            attendance.Date = dto.Date;
            attendance.IsPresent = dto.IsPresent;
            attendance.Remarks = dto.Remarks;

            await _context.SaveChangesAsync();

            return true;
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

        public async Task<double>
            GetAttendancePercentageAsync(int studentId)
        {
            var totalDays = await _context.Attendances
                .CountAsync(a => a.StudentId == studentId);

            if (totalDays == 0)
                return 0;

            var presentDays = await _context.Attendances
                .CountAsync(a =>
                    a.StudentId == studentId &&
                    a.IsPresent);

            return Math.Round(
                (double)presentDays / totalDays * 100,
                2);
        }

        public async Task<double>
            GetClassAttendanceAverageAsync()
        {
            var totalRecords =
                await _context.Attendances.CountAsync();

            if (totalRecords == 0)
                return 0;

            var presentRecords =
                await _context.Attendances
                    .CountAsync(a => a.IsPresent);

            return Math.Round(
                (double)presentRecords /
                totalRecords * 100,
                2);
        }

        public async Task<object>
            GetDailyReportAsync(DateTime date)
        {
            var records = await _context.Attendances
                .Where(a => a.Date.Date == date.Date)
                .ToListAsync();

            return new
            {
                Date = date,
                TotalStudents = records.Count,
                Present = records.Count(x => x.IsPresent),
                Absent = records.Count(x => !x.IsPresent)
            };
        }

        public async Task<object>
            GetMonthlyReportAsync(
                int month,
                int year)
        {
            var records = await _context.Attendances
                .Where(a =>
                    a.Date.Month == month &&
                    a.Date.Year == year)
                .ToListAsync();

            return new
            {
                Month = month,
                Year = year,
                TotalRecords = records.Count,
                Present = records.Count(x => x.IsPresent),
                Absent = records.Count(x => !x.IsPresent)
            };
        }
    }
}