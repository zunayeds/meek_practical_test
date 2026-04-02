using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IStudentRepository> _mockStudentRepository = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var mockStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            mockStore.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Enumerable.Empty<IUserValidator<ApplicationUser>>(),
            Enumerable.Empty<IPasswordValidator<ApplicationUser>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<ApplicationUser>>>());

        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepository.Object);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "32-chars-long-secret-key-for-jwt",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        _authService = new AuthService(_mockUserManager.Object, config, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        _mockUserManager.Setup(m => m.FindByNameAsync("unknown@outlook.com")).ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authService.AuthenticateAsync(new AuthenticationRequest { UserName = "unknown", Password = "wrong@Pass1" });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_ReturnsNull()
    {
        // Arrange
        var user = new ApplicationUser { Email = "admin@outlook.com", FirstName = "Admin", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByNameAsync("admin@outlook.com")).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);

        // Act
        var result = await _authService.AuthenticateAsync(new AuthenticationRequest { UserName = "admin@outlook.com", Password = "wrong@Pass1" });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ValidStaff_ReturnsTokenWithEmailAndRole()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "admin@outlook.com", FirstName = "Admin", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByNameAsync("admin@outlook.com")).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "Admin@123")).ReturnsAsync(true);
        _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync([Roles.Staff]);

        // Act
        var result = await _authService.AuthenticateAsync(new AuthenticationRequest { UserName = "admin@outlook.com", Password = "Admin@123" });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("admin@outlook.com", result.Email);
        Assert.Contains(Roles.Staff, result.Roles);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task AuthenticateAsync_StudentUser_CallsGetIdByUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, Email = "student@outlook.com", FirstName = "Student", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByNameAsync("student@outlook.com")).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "Pass@123")).ReturnsAsync(true);
        _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync([Roles.Student]);
        _mockStudentRepository.Setup(r => r.GetIdByUserIdAsync(userId, default)).ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _authService.AuthenticateAsync(new AuthenticationRequest { UserName = "student@outlook.com", Password = "Pass@123" });

        // Assert
        Assert.NotNull(result);
        _mockStudentRepository.Verify(r => r.GetIdByUserIdAsync(userId, default), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_StaffUser_NeverCallsGetIdByUserId()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "staff@outlook.com", FirstName = "Staff", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByNameAsync("staff@outlook.com")).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.CheckPasswordAsync(user, "Pass@123")).ReturnsAsync(true);
        _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync([Roles.Staff]);

        // Act
        await _authService.AuthenticateAsync(new AuthenticationRequest { UserName = "staff@outlook.com", Password = "Pass@123" });

        // Assert
        _mockStudentRepository.Verify(r => r.GetIdByUserIdAsync(It.IsAny<Guid>(), default), Times.Never);
    }
}

