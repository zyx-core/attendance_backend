using Microsoft.AspNetCore.Mvc;
using StudentAttendance.DTOs;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(
            IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(
            [FromBody] CreateAttendanceDto dto)
        {
            try
            {
                var result =
                    await _attendanceService.MarkAttendanceAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new { message = ex.Message });
            }
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetHistory(
            int studentId)
        {
            var result =
                await _attendanceService
                    .GetAttendanceHistoryAsync(studentId);

            return Ok(result);
        }

        [HttpGet("percentage/{studentId}")]
        public async Task<IActionResult> GetPercentage(
            int studentId)
        {
            var percentage =
                await _attendanceService
                    .GetAttendancePercentageAsync(studentId);

            return Ok(new
            {
                studentId,
                attendancePercentage = percentage
            });
        }
    }
}