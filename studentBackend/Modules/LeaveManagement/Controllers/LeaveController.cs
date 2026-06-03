using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.Modules.LeaveManagement.DTOs;
using StudentAttendance.Modules.LeaveManagement.Services;

namespace StudentAttendance.Modules.LeaveManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost]
    [Authorize(Policy = "ParentOnly")]
    public async Task<IActionResult> CreateLeaveRequest([FromBody] LeaveRequestDto dto)
    {
        // Mock getting parent ID from claims
        var parentId = 1; 
        var result = await _leaveService.CreateLeaveRequestAsync(parentId, dto);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetLeaveRequests()
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value ?? "Teacher";
        
        if (role == "Parent")
        {
            var parentId = 1; // Mock
            var result = await _leaveService.GetLeaveRequestsByParentAsync(parentId);
            return Ok(result);
        }
        else
        {
            var result = await _leaveService.GetAllLeaveRequestsAsync();
            return Ok(result);
        }
    }

    [HttpPut("{id}/approve")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> ApproveLeave(int id)
    {
        var teacherId = 2; // Mock
        var result = await _leaveService.ApproveLeaveAsync(id, teacherId);
        return Ok(result);
    }

    [HttpPut("{id}/reject")]
    [Authorize(Policy = "TeacherOnly")]
    public async Task<IActionResult> RejectLeave(int id)
    {
        var teacherId = 2; // Mock
        var result = await _leaveService.RejectLeaveAsync(id, teacherId);
        return Ok(result);
    }
}
