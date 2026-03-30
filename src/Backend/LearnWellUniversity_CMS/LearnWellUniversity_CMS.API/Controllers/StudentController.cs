using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentController(IStudentService studentService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Create([FromBody] CreateUpdateStudentRequest request)
    {
        var student = await studentService.CreateAsync(request);
        return Ok(student);
    }

    [HttpGet]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetAll([FromQuery] GetStudentByFiltersRequest request, CancellationToken cancellationToken = default)
    {
        var students = await studentService.GetStudentsAsync(request, cancellationToken);
        return Ok(students);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.StaffOrStudent)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await studentService.GetByStudentIdAsync(id, cancellationToken);
        return Ok(student);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.StaffOrStudent)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        var student = await studentService.UpdateStudentAsync(id, request, cancellationToken);
        return Ok(student);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await studentService.DeleteStudentByIdAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("getOtherStudents/{classId:guid}")]
    [Authorize(Policy = Policies.StudentOnly)]
    public async Task<IActionResult> GetOtherStudents(Guid classId, CancellationToken cancellationToken = default)
    {
        var studentNames = await studentService.GetOtherStudentNamesByClassIdAsync(classId, cancellationToken);
        return Ok(studentNames);
    }

    [HttpGet("getClasses/{studentId:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetClasses(Guid studentId, CancellationToken cancellationToken = default)
    {
        var classes = await studentService.GetClassesByStudentIdAsync(studentId, cancellationToken);
        return Ok(classes);
    }
}
