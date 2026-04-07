using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policies.StaffOnly)]
public class StaffController(IUserService userService, ILogger<StaffController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddStaffAsync([FromBody] CreateUpdateUserRequest request)
    {
        logger.LogInformation("Adding staff '{FirstName} {LastName}'", request.FirstName, request.LastName);
        var result = await userService.CreateWithRoleAsync(request, Roles.Staff);
        logger.LogInformation("Added staff '{FirstName} {LastName}'", request.FirstName, request.LastName);
        return Ok(result);
    }
}
