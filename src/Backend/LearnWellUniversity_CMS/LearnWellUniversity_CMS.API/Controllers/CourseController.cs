using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseController(ICourseService courseService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Create([FromBody] CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = await courseService.CreateAsync(request, cancellationToken);
        return Ok(course);
    }

    [HttpGet]
    [Authorize(Policy = Policies.StaffOrStudent)]
    public async Task<IActionResult> GetAll([FromQuery] GetByFiltersBaseRequest request, CancellationToken cancellationToken = default)
    {
        var courses = await courseService.GetCoursesAsync(request, cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await courseService.GetByCourseIdAsync(id, cancellationToken);
        return Ok(course);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = await courseService.UpdateCourseAsync(id, request, cancellationToken);
        return Ok(course);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await courseService.DeleteCourseByIdAsync(id, cancellationToken);
        return NoContent();
    }
}
