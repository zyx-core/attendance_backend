using Microsoft.AspNetCore.Mvc;
using studentBackend.DTOs;
using studentBackend.Services;

namespace studentBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentListDto>>> GetStudents()
    {
        var students = await _studentService.GetStudentsAsync();
        return Ok(students);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentResponseDto>> GetStudentById(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> CreateStudent(StudentCreateDto studentCreateDto)
    {
        var student = await _studentService.CreateStudentAsync(studentCreateDto);
        return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StudentResponseDto>> UpdateStudent(int id, StudentUpdateDto studentUpdateDto)
    {
        var student = await _studentService.UpdateStudentAsync(id, studentUpdateDto);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var deleted = await _studentService.DeleteStudentAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<StudentListDto>>> SearchStudents([FromQuery] string keyword = "")
    {
        var students = await _studentService.SearchStudentsAsync(keyword);
        return Ok(students);
    }
}
