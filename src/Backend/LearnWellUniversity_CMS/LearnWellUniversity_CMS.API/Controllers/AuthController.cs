using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticationRequest request)
    {
        var result = await authService.AuthenticateAsync(request);
        if (result is null)
        {
            return Unauthorized(new { Message = "Invalid username or password." });
        }
        return Ok(result);
    }
}
