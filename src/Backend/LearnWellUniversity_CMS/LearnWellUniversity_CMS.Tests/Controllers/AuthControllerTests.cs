using LearnWellUniversity_CMS.API.Controllers;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService = new();
    private readonly AuthController _authController;

    public AuthControllerTests()
        => _authController = new AuthController(_mockAuthService.Object, new Mock<ILogger<AuthController>>().Object);

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsOkWithToken()
    {
        var request = new AuthenticationRequest { UserName = "admin@learnwell.edu", Password = "Admin@123" };
        _mockAuthService.Setup(s => s.AuthenticateAsync(request, default))
                    .ReturnsAsync(new AuthenticationResponse { Token = "jwt.token", Email = "admin@learnwell.edu" });

        var result = await _authController.AuthenticateAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotEmpty(((AuthenticationResponse)ok.Value!).Token);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_ReturnsUnauthorized()
    {
        var request = new AuthenticationRequest { UserName = "john@outlook.com", Password = "wrongPass" };

        _mockAuthService.Setup(s => s.AuthenticateAsync(request, default)).ReturnsAsync((AuthenticationResponse?)null);

        Assert.IsType<UnauthorizedObjectResult>(await _authController.AuthenticateAsync(request));
    }
}
