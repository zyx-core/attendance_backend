using StudentAttendance.Data.Entities;

namespace StudentAttendance.Modules.LeaveManagement.Repositories;

public interface ILeaveRepository
{
    Task<LeaveRequest> CreateAsync(LeaveRequest request);
    Task<LeaveRequest?> GetByIdAsync(int id);
    Task<IEnumerable<LeaveRequest>> GetAllAsync();
    Task<IEnumerable<LeaveRequest>> GetByStudentIdAsync(int studentId);
    Task<IEnumerable<LeaveRequest>> GetByParentIdAsync(int parentId);
    Task UpdateAsync(LeaveRequest request);
}
