using LearnWellUniversity_CMS.API.Controllers;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Constants;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Controllers;

public class StaffControllerTests
{
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly StaffController _staffController;

    public StaffControllerTests()
        => _staffController = new StaffController(_mockUserService.Object, new Mock<ILogger<StaffController>>().Object);

    private CreateUpdateUserRequest ValidUserCreateUpdateRequest = new()
    {
        FirstName = "Joe",
        LastName = "Smith",
        EmailAddress = "alice@gmail.com",
        PhoneNumber = "1234567890"
    };

    [Fact]
    public async Task AddStaff_ValidRequest_ReturnsOkWithIdAndPassword()
    {
        var expectedId = Guid.NewGuid();
        _mockUserService.Setup(s => s.CreateWithRoleAsync(ValidUserCreateUpdateRequest, Roles.Staff))
                    .ReturnsAsync((expectedId, "Generated@Pass1"));

        var result = await _staffController.AddStaffAsync(ValidUserCreateUpdateRequest);

        var ok = Assert.IsType<OkObjectResult>(result);
        var (id, password) = ((Guid, string))ok.Value!;
        Assert.Equal(expectedId, id);
        Assert.NotEmpty(password);
    }

    [Fact]
    public async Task AddStaff_AlwaysPassesStaffRoleToService()
    {
        _mockUserService.Setup(s => s.CreateWithRoleAsync(ValidUserCreateUpdateRequest, Roles.Staff)).ReturnsAsync((Guid.NewGuid(), "Generated@Pass1"));

        await _staffController.AddStaffAsync(ValidUserCreateUpdateRequest);

        _mockUserService.Verify(s => s.CreateWithRoleAsync(ValidUserCreateUpdateRequest, Roles.Staff), Times.Once);
        _mockUserService.Verify(s => s.CreateWithRoleAsync(It.IsAny<CreateUpdateUserRequest>(), Roles.Student), Times.Never);
    }

    [Fact]
    public async Task AddStaff_DuplicateEmail_PropagatesAlreadyExistException()
    {
        _mockUserService.Setup(s => s.CreateWithRoleAsync(It.IsAny<CreateUpdateUserRequest>(), Roles.Staff))
                    .ThrowsAsync(new AlreadyExistException("exists"));
        await Assert.ThrowsAsync<AlreadyExistException>(() =>
            _staffController.AddStaffAsync(ValidUserCreateUpdateRequest));
    }
}
