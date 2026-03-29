using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClassController(IClassService classService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request, CancellationToken cancellationToken = default)
    {
        var @class = await classService.CreateAsync(request, cancellationToken);
        return Ok(@class);
    }

    [HttpGet]
    [Authorize(Policy = Policies.StaffOrStudent)]
    public async Task<IActionResult> GetAll([FromQuery] GetByFiltersBaseRequest request, CancellationToken cancellationToken = default)
    {
        var courses = await classService.GetClasssAsync(request, cancellationToken);
        return Ok(courses);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var course = await classService.GetByClassIdAsync(id, cancellationToken);
        return Ok(course);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateClassRequest request, CancellationToken cancellationToken = default)
    {
        var course = await classService.UpdateClassAsync(id, request, cancellationToken);
        return Ok(course);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await classService.DeleteClassByIdAsync(id, cancellationToken);
        return NoContent();
    }
}
