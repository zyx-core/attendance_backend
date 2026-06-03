using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentAttendance.DTOs;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterTeacherDto dto)
        {
            var success = await _authService.RegisterTeacherAsync(dto);
            if (success)
                return Ok(new { Message = "Teacher registered successfully." });

            return BadRequest(new { Message = "Registration failed. Email might already exist." });
        }

        [HttpPost("register/parent")]
        public async Task<IActionResult> RegisterParent([FromBody] AcceptInvitationDto dto)
        {
            var errorMessage = await _authService.RegisterParentAsync(dto);
            if (errorMessage == null)
                return Ok(new { Message = "Parent registered successfully." });

            return BadRequest(new { Message = errorMessage });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token != null)
                return Ok(new { Token = token, Message = "Login successful." });

            return Unauthorized(new { Message = "Invalid email or password." });
        }
    }
}
