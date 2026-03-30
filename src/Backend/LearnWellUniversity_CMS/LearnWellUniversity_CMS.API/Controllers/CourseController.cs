using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CourseController(ICourseService courseService, ILogger<ClassController> logger) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Create([FromBody] CreateUpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating course '{Name}'", request.Name);
        var course = await courseService.CreateAsync(request, cancellationToken);
        logger.LogInformation("Successfully created course '{Name}'", request.Name);
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
        logger.LogInformation("Updating course with id '{id}'", id);
        var course = await courseService.UpdateCourseAsync(id, request, cancellationToken);
        logger.LogInformation("Updated course with id '{id}'", id);
        return Ok(course);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting course with id '{id}'", id);
        await courseService.DeleteCourseByIdAsync(id, cancellationToken);
        logger.LogWarning("Deleted course with id '{id}'", id);
        return NoContent();
    }

    [HttpPost("addRemoveStudents/{courseId:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> AddRemoveStudents(Guid courseId, [FromBody] AddRemoveStudentsRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding/removing students from course with id '{courseId}'", courseId);
        await courseService.AddRemoveStudentsInCourseAsync(courseId, request, cancellationToken);
        logger.LogInformation("Added/removed students from course with id '{courseId}'", courseId);
        return NoContent();
    }

    [HttpGet("getStudents/{courseId:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetStudents(Guid courseId, CancellationToken cancellationToken = default)
    {
        var students = await courseService.GetStudentsInCourseAsync(courseId, cancellationToken);
        return Ok(students);
    }

    [HttpGet("getClasses/{courseId:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetClasses(Guid courseId, CancellationToken cancellationToken = default)
    {
        var classes = await courseService.GetClassesInCourseAsync(courseId, cancellationToken);
        return Ok(classes);
    }

    [HttpPost("addRemoveClasses/{courseId:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> AddRemoveClasses(Guid courseId, [FromBody] AddRemoveClassessRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding/removing classes from course with id '{courseId}'", courseId);
        await courseService.AddRemoveClassesInCourseAsync(courseId, request, cancellationToken);
        logger.LogInformation("Added/removed classes from course with id '{courseId}'", courseId);
        return NoContent();
    }
}
