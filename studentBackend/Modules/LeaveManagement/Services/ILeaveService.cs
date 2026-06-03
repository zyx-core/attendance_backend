using StudentAttendance.Data.Entities;
using StudentAttendance.Modules.LeaveManagement.DTOs;

namespace StudentAttendance.Modules.LeaveManagement.Services;

public interface ILeaveService
{
    Task<LeaveResponseDto> CreateLeaveRequestAsync(int parentId, LeaveRequestDto dto);
    Task<IEnumerable<LeaveResponseDto>> GetAllLeaveRequestsAsync();
    Task<IEnumerable<LeaveResponseDto>> GetLeaveRequestsByParentAsync(int parentId);
    Task<LeaveResponseDto> ApproveLeaveAsync(int id, int reviewerId);
    Task<LeaveResponseDto> RejectLeaveAsync(int id, int reviewerId);
}
