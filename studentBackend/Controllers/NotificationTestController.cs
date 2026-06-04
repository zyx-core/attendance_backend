using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTestController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public NotificationTestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("test-absence")]
        public async Task<IActionResult> TestAbsenceEmail([FromQuery] string email, [FromQuery] string studentName)
        {
            await _emailService.SendAbsentNotificationAsync(1, email, studentName);
            return Ok(new { message = $"Absence simulation test email dispatched to {email}." });
        }

        [HttpPost("test-warning")]
        public async Task<IActionResult> TestWarningEmail([FromQuery] string email, [FromQuery] string studentName, [FromQuery] decimal percentage)
        {
            await _emailService.SendAttendanceWarningAsync(1, email, studentName, percentage);
            return Ok(new { message = $"Attendance threshold alert dispatched to {email}." });
        }
    }
}