using StudentAttendance.Data.Entities;
using StudentAttendance.Modules.LeaveManagement.DTOs;
using StudentAttendance.Modules.LeaveManagement.Repositories;
using StudentAttendance.Modules.Notifications.Services;

namespace StudentAttendance.Modules.LeaveManagement.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepository;
    private readonly IEmailService _emailService;

    public LeaveService(ILeaveRepository leaveRepository, IEmailService emailService)
    {
        _leaveRepository = leaveRepository;
        _emailService = emailService;
    }

    public async Task<LeaveResponseDto> CreateLeaveRequestAsync(int parentId, LeaveRequestDto dto)
    {
        var request = new LeaveRequest
        {
            StudentId = dto.StudentId,
            ParentId = parentId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending
        };

        var created = await _leaveRepository.CreateAsync(request);
        var fullRequest = await _leaveRepository.GetByIdAsync(created.Id);
        return MapToDto(fullRequest!);
    }

    public async Task<IEnumerable<LeaveResponseDto>> GetAllLeaveRequestsAsync()
    {
        var requests = await _leaveRepository.GetAllAsync();
        return requests.Select(MapToDto);
    }

    public async Task<IEnumerable<LeaveResponseDto>> GetLeaveRequestsByParentAsync(int parentId)
    {
        var requests = await _leaveRepository.GetByParentIdAsync(parentId);
        return requests.Select(MapToDto);
    }

    public async Task<LeaveResponseDto> ApproveLeaveAsync(int id, int reviewerId)
    {
        var request = await _leaveRepository.GetByIdAsync(id);
        if (request == null) throw new Exception("Leave request not found");

        request.Status = LeaveStatus.Approved;
        request.ReviewedBy = reviewerId;
        request.ReviewedAt = DateTime.UtcNow;

        await _leaveRepository.UpdateAsync(request);
        
        if (request.Parent != null && request.Student != null)
        {
            await _emailService.SendLeaveApprovedAsync(request.Parent.Email, request.Student.FirstName, request.StartDate, request.EndDate);
        }

        return MapToDto(request);
    }

    public async Task<LeaveResponseDto> RejectLeaveAsync(int id, int reviewerId)
    {
        var request = await _leaveRepository.GetByIdAsync(id);
        if (request == null) throw new Exception("Leave request not found");

        request.Status = LeaveStatus.Rejected;
        request.ReviewedBy = reviewerId;
        request.ReviewedAt = DateTime.UtcNow;

        await _leaveRepository.UpdateAsync(request);
        
        if (request.Parent != null && request.Student != null)
        {
            await _emailService.SendLeaveRejectedAsync(request.Parent.Email, request.Student.FirstName, request.StartDate, request.EndDate);
        }

        return MapToDto(request);
    }

    private static LeaveResponseDto MapToDto(LeaveRequest r)
    {
        return new LeaveResponseDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            StudentName = r.Student != null ? $"{r.Student.FirstName} {r.Student.LastName}".Trim() : "Unknown",
            ParentId = r.ParentId,
            ParentName = r.Parent != null ? r.Parent.FullName : "Unknown",
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Reason = r.Reason,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt
        };
    }
}
