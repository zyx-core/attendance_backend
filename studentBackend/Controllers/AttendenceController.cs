using Microsoft.AspNetCore.Mvc;
using StudentAttendance.DTOs;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService
            _attendanceService;

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
                    await _attendanceService
                        .MarkAttendanceAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>
            UpdateAttendance(
                int id,
                [FromBody] CreateAttendanceDto dto)
        {
            var updated =
                await _attendanceService
                    .UpdateAttendanceAsync(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Attendance record not found"
                });
            }

            return Ok(new
            {
                message =
                    "Attendance updated successfully"
            });
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult>
            GetAttendanceHistory(int studentId)
        {
            var result =
                await _attendanceService
                    .GetAttendanceHistoryAsync(studentId);

            return Ok(result);
        }

        [HttpGet("percentage/{studentId}")]
        public async Task<IActionResult>
            GetAttendancePercentage(int studentId)
        {
            var percentage =
                await _attendanceService
                    .GetAttendancePercentageAsync(studentId);

            return Ok(new
            {
                StudentId = studentId,
                AttendancePercentage = percentage
            });
        }

        [HttpGet("class-average")]
        public async Task<IActionResult>
            GetClassAverage()
        {
            var average =
                await _attendanceService
                    .GetClassAttendanceAverageAsync();

            return Ok(new
            {
                ClassAttendanceAverage = average
            });
        }

        [HttpGet("daily")]
        public async Task<IActionResult>
            GetDailyReport(DateTime date)
        {
            var report =
                await _attendanceService
                    .GetDailyReportAsync(date);

            return Ok(report);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult>
            GetMonthlyReport(
                int month,
                int year)
        {
            var report =
                await _attendanceService
                    .GetMonthlyReportAsync(month, year);

            return Ok(report);
        }
    }
}