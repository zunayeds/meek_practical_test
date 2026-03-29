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
    public async Task<IActionResult> Create([FromBody] CreateClassRequest request)
    {
        var @class = await classService.CreateAsync(request);
        return Ok(@class);
    }
}
