using System.Collections.Generic;
using System.Threading.Tasks;
using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface ILeaveService
    {
        Task<LeaveRequestResponseDto> CreateLeaveAsync(CreateLeaveRequestDto dto);
        Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeavesAsync();
        Task<bool> ProcessLeaveAsync(int id, string reviewer, bool isApproved);
    }
}