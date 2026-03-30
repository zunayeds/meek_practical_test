using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        var result = await authService.AuthenticateAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { Message = "Invalid username or password." });
        }
        return Ok(result);
    }
}
