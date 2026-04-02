using AutoMapper;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            mockUserStore.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Enumerable.Empty<IUserValidator<ApplicationUser>>(),
            Enumerable.Empty<IPasswordValidator<ApplicationUser>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<ApplicationUser>>>());
        _userService = new UserService(_mockUserManager.Object, _mockMapper.Object, new Mock<ILogger<UserService>>().Object);
    }

    #region CreateWithRoleAsync
    
    [Fact]
    public async Task CreateWithRoleAsync_UserExists_ThrowErrorIfExists_ThrowsAlreadyExistException()
    {
        // Arrange
        var user = new ApplicationUser { Email = "existing@gmail.com", FirstName = "Existing", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByEmailAsync(user.Email!))
                        .ReturnsAsync(new ApplicationUser { Id = Guid.NewGuid(), FirstName = "Existing", LastName = "User" });

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _userService.CreateWithRoleAsync(user, Roles.Staff, null, throwErrorIfExists: true));
    }

    [Fact]
    public async Task CreateWithRoleAsync_UserExists_NoThrow_ReturnsExistingId()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var user = new ApplicationUser { Email = "existing@gmail.com", FirstName = "Existing", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByEmailAsync(user.Email!))
                        .ReturnsAsync(new ApplicationUser { Id = existingId, FirstName = "Existing", LastName = "User" });

        // Act
        var (id, password) = await _userService.CreateWithRoleAsync(user, Roles.Staff, "pass", throwErrorIfExists: false);

        // Assert
        Assert.Equal(existingId, id);
    }

    [Fact]
    public async Task CreateWithRoleAsync_NewUser_CreatesUserAndAssignsRole()
    {
        // Arrange
        var user = new ApplicationUser { Email = "new@gmail.com", FirstName = "New", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(m => m.CreateAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(m => m.AddToRoleAsync(user, Roles.Staff)).ReturnsAsync(IdentityResult.Success);

        // Act
        var (id, password) = await _userService.CreateWithRoleAsync(user, Roles.Staff, null);

        // Assert
        Assert.NotEmpty(password);
        _mockUserManager.Verify(m => m.CreateAsync(user, It.IsAny<string>()), Times.Once);
        _mockUserManager.Verify(m => m.AddToRoleAsync(user, Roles.Staff), Times.Once);
    }

    [Fact]
    public async Task CreateWithRoleAsync_Request_MapsAndDelegates()
    {
        // Arrange
        var request = new CreateUpdateUserRequest
        {
            FirstName = "Map",
            LastName = "Test",
            EmailAddress = "map@gmail.com",
            PhoneNumber = "555"
        };
        var mappedUser = new ApplicationUser { Email = "map@gmail.com", FirstName = "Map", LastName = "Test" };
        _mockMapper.Setup(m => m.Map<ApplicationUser>(request)).Returns(mappedUser);
        _mockUserManager.Setup(m => m.FindByEmailAsync(mappedUser.Email!)).ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(m => m.CreateAsync(mappedUser, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(m => m.AddToRoleAsync(mappedUser, Roles.Staff)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.CreateWithRoleAsync(request, Roles.Staff);

        // Assert
        _mockMapper.Verify(m => m.Map<ApplicationUser>(request), Times.Once);
    }

    #endregion

    #region DeleteUserByIdAsync
    
    [Fact]
    public async Task DeleteUserByIdAsync_UserFound_DeletesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, FirstName = "Test", LastName = "User" };
        _mockUserManager.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.DeleteUserByIdAsync(userId);

        // Assert
        _mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUserByIdAsync_UserNotFound_DoesNotCallDelete()
    {
        // Arrange
        _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        // Act
        await _userService.DeleteUserByIdAsync(Guid.NewGuid());

        // Assert
        _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    #endregion
}
