using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.DTOs;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
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
        [Authorize(Policy = "AllRoles")] 
        public async Task<IActionResult> CreateLeave([FromBody] CreateLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _leaveService.CreateLeaveAsync(dto);
            return CreatedAtAction(nameof(GetLeaves), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize(Policy = "TeacherOnly")] 
        public async Task<IActionResult> GetLeaves()
        {
            var result = await _leaveService.GetAllLeavesAsync();
            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Policy = "TeacherOnly")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var reviewer = User.Identity?.Name ?? "Authenticated Teacher";
            var success = await _leaveService.ProcessLeaveAsync(id, reviewer, isApproved: true);
            
            if (!success)
            {
                return BadRequest(new { message = "Leave request not found or has already been processed." });
            }

            return Ok(new { message = "Leave request approved successfully." });
        }

        [HttpPut("{id}/reject")]
        [Authorize(Policy = "TeacherOnly")]
        public async Task<IActionResult> RejectLeave(int id)
        {
            var reviewer = User.Identity?.Name ?? "Authenticated Teacher";
            var success = await _leaveService.ProcessLeaveAsync(id, reviewer, isApproved: false);
            
            if (!success)
            {
                return BadRequest(new { message = "Leave request not found or has already been processed." });
            }

            return Ok(new { message = "Leave request rejected successfully." });
        }
    }
}