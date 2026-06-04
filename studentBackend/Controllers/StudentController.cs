using Microsoft.AspNetCore.Mvc;
using StudentAttendance.DTOs;
using StudentAttendance.Services;

namespace StudentAttendance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentService.GetStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

            return Ok(student);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudents(string? keyword, string? className, string? status)
        {
            var students = await _studentService.SearchStudentsAsync(keyword, className, status);
            return Ok(students);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _studentService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentUpdateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _studentService.UpdateStudentAsync(id, dto);
            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Student not found" });
            }

            return NoContent();
        }

        [HttpPost("upload-photo/{id}")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
        {
            if (file == null)
            {
                return BadRequest(new { message = "Photo file is required" });
            }

            try
            {
                var student = await _studentService.UploadPhotoAsync(id, file, Request);
                if (student == null)
                {
                    return NotFound(new { message = "Student not found" });
                }

                return Ok(student);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
