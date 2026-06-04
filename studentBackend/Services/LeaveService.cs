using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentAttendance.DTOs;
using StudentAttendance.Models;

namespace StudentAttendance.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly AppDbContext _context;

        public LeaveService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequestResponseDto> CreateLeaveAsync(CreateLeaveRequestDto dto)
        {
            var leave = new LeaveRequest
            {
                StudentId = dto.StudentId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = LeaveStatus.Pending
            };

            await _context.LeaveRequests.AddAsync(leave);
            await _context.SaveChangesAsync();

            return MapToDto(leave);
        }

        public async Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeavesAsync()
        {
            var leaves = await _context.LeaveRequests.ToListAsync();
            return leaves.Select(MapToDto);
        }

        public async Task<bool> ProcessLeaveAsync(int id, string reviewer, bool isApproved)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave == null || leave.Status != LeaveStatus.Pending)
            {
                return false;
            }

            leave.Status = isApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
            leave.ReviewedBy = reviewer;
            leave.ReviewedAt = DateTime.UtcNow;

            _context.LeaveRequests.Update(leave);
            await _context.SaveChangesAsync();

            return true;
        }

        private static LeaveRequestResponseDto MapToDto(LeaveRequest leave) => new()
        {
            Id = leave.Id,
            StudentId = leave.StudentId,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            Status = leave.Status.ToString(),
            ReviewedBy = leave.ReviewedBy,
            ReviewedAt = leave.ReviewedAt,
            CreatedAt = leave.CreatedAt
        };
    }
}
