using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearnWellUniversity_CMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting login with {UserName}", request.UserName);
        var result = await authService.AuthenticateAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { Message = "Invalid username or password." });
        }
        logger.LogInformation("Successful login with {UserName}", request.UserName);
        return Ok(result);
    }
}
