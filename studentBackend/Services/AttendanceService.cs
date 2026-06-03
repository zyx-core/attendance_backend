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

        public async Task<AttendanceResponseDto> MarkAttendanceAsync(CreateAttendanceDto dto)
        {
            var exists = await _context.Attendances
                .AnyAsync(a =>
                    a.StudentId == dto.StudentId &&
                    a.Date.Date == dto.Date.Date);

            if (exists)
            {
                throw new Exception("Attendance already marked for this date.");
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

        public async Task<IEnumerable<AttendanceResponseDto>> GetAttendanceHistoryAsync(int studentId)
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

        public async Task<double> GetAttendancePercentageAsync(int studentId)
        {
            var totalDays = await _context.Attendances
                .CountAsync(a => a.StudentId == studentId);

            if (totalDays == 0)
                return 0;

            var presentDays = await _context.Attendances
                .CountAsync(a =>
                    a.StudentId == studentId &&
                    a.IsPresent);

            return Math.Round((double)presentDays / totalDays * 100, 2);
        }
    }
}