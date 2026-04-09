using LearnWellUniversity_CMS.API.Controllers;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Controllers;

public class StudentControllerTests
{
    private readonly Mock<IStudentService> _mockStudentService = new();
    private readonly StudentController _studentController;

    public StudentControllerTests()
        => _studentController = new StudentController(_mockStudentService.Object, new Mock<ILogger<StudentController>>().Object);

    private readonly CreateUpdateStudentRequest ValidStudentCreateUpdateRequest = new()
    {
        FirstName = "Alice",
        LastName = "Smith",
        EmailAddress = "alice@gmail.com",
        PhoneNumber = "1234567890",
        Address = "Address"
    };

    [Fact]
    public async Task Create_ValidRequest_ReturnsOkWithPassword()
    {
        // Arrange
        _mockStudentService.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ReturnsAsync(new CreateStudentResponse { Id = Guid.NewGuid(), Password = "Generated@Pass1" });

        // Act
        var result = await _studentController.Create(ValidStudentCreateUpdateRequest);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotEmpty(((CreateStudentResponse)ok.Value!).Password);
    }

    [Fact]
    public async Task Create_DuplicateEmailOrPhone_PropagatesAlreadyExistException()
    {
        // Arrange
        _mockStudentService.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ThrowsAsync(new AlreadyExistException("exists"));

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _studentController.Create(ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetStudentsAsync(It.IsAny<GetStudentByFiltersRequest>(), default))
                    .ReturnsAsync(new PaginatedResponse<StudentResponseBase>
                    {
                        TotalRecords = 1,
                        Records = [new StudentResponseBase { FirstName = "Alice" }]
                    });

        // Act
        var result = await _studentController.GetAll(new GetStudentByFiltersRequest());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockStudentService.Setup(s => s.GetByStudentIdAsync(id, default)).ReturnsAsync(new StudentResponse { StudentId = id });

        // Act
        var result = await _studentController.GetById(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_StudentNotFound_PropagatesNotFoundException()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetByStudentIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentController.GetById(Guid.NewGuid()));
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockStudentService.Setup(s => s.UpdateStudentAsync(id, It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ReturnsAsync(new StudentResponse { StudentId = id });

        // Act
        var result = await _studentController.Update(id, ValidStudentCreateUpdateRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_StudentNotFound_PropagatesNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockStudentService.Setup(s => s.UpdateStudentAsync(id, It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentController.Update(id, ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        // Arrange
        _mockStudentService.Setup(s => s.DeleteStudentByIdAsync(It.IsAny<Guid>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _studentController.Delete(Guid.NewGuid());

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_StudentNotFound_PropagatesNotFoundException()
    {
        // Arrange
        _mockStudentService.Setup(s => s.DeleteStudentByIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentController.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetOtherStudents_ReturnsOkWithNameList()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetOtherStudentNamesByClassIdAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync(["Alice Smith"]);

        // Act
        var result = await _studentController.GetOtherStudents(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetClassesByStudentId_ReturnsOk()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetClassesByStudentIdAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new StudentClassResponse { Name = "Math 101" }]);

        // Act
        var result = await _studentController.GetClassesByStudentId(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetClasses_ReturnsOk()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetClassesAsync(default))
                    .ReturnsAsync([new StudentClassResponse { Name = "Science 101" }]);

        // Act
        var result = await _studentController.GetClasses();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetCoursesByStudentId_ReturnsOk()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetCoursesByStudentIdAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new StudentCourseResponse { Name = "Mathematics" }]);

        // Act
        var result = await _studentController.GetCoursesByStudentId(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetCourses_ReturnsOk()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetCoursesAsync(default))
                    .ReturnsAsync([new StudentCourseResponse { Name = "Business" }]);

        // Act
        var result = await _studentController.GetCourses();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetOwnInfo_ReturnsOk()
    {
        // Arrange
        _mockStudentService.Setup(s => s.GetOwnInfoAsync(default))
                    .ReturnsAsync(new StudentOwnResponse { FirstName = "Jane" });

        // Act
        var result = await _studentController.GetOwnInfo();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
