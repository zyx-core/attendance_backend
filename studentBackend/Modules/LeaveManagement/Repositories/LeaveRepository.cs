using Microsoft.EntityFrameworkCore;
using StudentAttendance.Data;
using StudentAttendance.Data.Entities;

namespace StudentAttendance.Modules.LeaveManagement.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext _context;

    public LeaveRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest> CreateAsync(LeaveRequest request)
    {
        _context.LeaveRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<LeaveRequest?> GetByIdAsync(int id)
    {
        return await _context.LeaveRequests
            .Include(l => l.Student)
            .Include(l => l.Parent)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
    {
        return await _context.LeaveRequests
            .Include(l => l.Student)
            .Include(l => l.Parent)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByStudentIdAsync(int studentId)
    {
        return await _context.LeaveRequests
            .Include(l => l.Student)
            .Include(l => l.Parent)
            .Where(l => l.StudentId == studentId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveRequest>> GetByParentIdAsync(int parentId)
    {
        return await _context.LeaveRequests
            .Include(l => l.Student)
            .Include(l => l.Parent)
            .Where(l => l.ParentId == parentId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(LeaveRequest request)
    {
        _context.LeaveRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}
