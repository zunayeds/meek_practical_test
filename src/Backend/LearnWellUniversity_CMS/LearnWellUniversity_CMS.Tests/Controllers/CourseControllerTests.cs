using LearnWellUniversity_CMS.API.Controllers;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Controllers;

public class CourseControllerTests
{
    private readonly Mock<ICourseService> _mockCourseService = new();
    private readonly CourseController _courseController;

    public CourseControllerTests()
        => _courseController = new CourseController(_mockCourseService.Object, new Mock<ILogger<CourseController>>().Object);

    [Fact]
    public async Task Create_ValidRequest_ReturnsOk()
    {
        // Arrange
        _mockCourseService.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateCourseRequest>(), default))
                    .ReturnsAsync(new CreatedEntityResponse { Id = Guid.NewGuid() });

        // Act
        var result = await _courseController.Create(new CreateUpdateCourseRequest { Name = "Physics 101" });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Create_DuplicateName_PropagatesAlreadyExistException()
    {
        // Arrange
        _mockCourseService.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateCourseRequest>(), default))
                    .ThrowsAsync(new AlreadyExistException("exists"));

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _courseController.Create(new CreateUpdateCourseRequest { Name = "X" }));
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetCoursesAsync(It.IsAny<GetByFiltersBaseRequest>(), default))
                    .ReturnsAsync([new CourseResponseBase { Name = "Physics 101" }]);

        // Act
        var result = await _courseController.GetAll(new GetByFiltersBaseRequest());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockCourseService.Setup(s => s.GetByCourseIdAsync(id, default)).ReturnsAsync(new CourseResponse { CourseId = id });

        // Act
        var result = await _courseController.GetById(id);

        // Act
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_CourseNotFound_PropagatesNotFoundException()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetByCourseIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseController.GetById(Guid.NewGuid()));
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockCourseService.Setup(s => s.UpdateCourseAsync(id, It.IsAny<CreateUpdateCourseRequest>(), default))
                    .ReturnsAsync(new CourseResponse { CourseId = id });

        // Act
        var result = await _courseController.Update(id, new CreateUpdateCourseRequest { Name = "New" });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_CourseNotFound_PropagatesNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockCourseService.Setup(s => s.UpdateCourseAsync(id, It.IsAny<CreateUpdateCourseRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseController.Update(id, new CreateUpdateCourseRequest { Name = "New" }));
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        // Arrange
        _mockCourseService.Setup(s => s.DeleteCourseByIdAsync(It.IsAny<Guid>(), default)).Returns(Task.CompletedTask);

        // Act
        var result = await _courseController.Delete(Guid.NewGuid());

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_CourseNotFound_PropagatesNotFoundException()
    {
        // Arrange
        _mockCourseService.Setup(s => s.DeleteCourseByIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseController.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task AddRemoveStudents_ReturnsNoContent()
    {
        // Arrange
        _mockCourseService.Setup(s => s.AddRemoveStudentsInCourseAsync(It.IsAny<Guid>(), It.IsAny<AddRemoveStudentsRequest>(), default))
                    .Returns(Task.CompletedTask);

        // Act
        var result = await _courseController.AddRemoveStudents(Guid.NewGuid(),
            new AddRemoveStudentsRequest { AddStudentIds = [], RemoveStudentIds = [] });

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddRemoveStudents_CourseNotFound_PropagatesNotFoundException()
    {
        // Arrange
        _mockCourseService.Setup(s => s.AddRemoveStudentsInCourseAsync(It.IsAny<Guid>(), It.IsAny<AddRemoveStudentsRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseController.AddRemoveStudents(Guid.NewGuid(),
            new AddRemoveStudentsRequest { AddStudentIds = [], RemoveStudentIds = [] }));
    }

    [Fact]
    public async Task GetStudents_ReturnsOkWithList()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetStudentsInCourseAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new StudentResponseBase { FirstName = "John" }]);

        // Act
        var result = await _courseController.GetStudents(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetClasses_ReturnsOkWithList()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetClassesInCourseAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new ClassResponseBase { Name = "Math 101" }]);

        // Act
        var result = await _courseController.GetClasses(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddRemoveClasses_ReturnsNoContent()
    {
        // Arrange
        _mockCourseService.Setup(s => s.AddRemoveClassesInCourseAsync(It.IsAny<Guid>(), It.IsAny<AddRemoveClassessRequest>(), default))
                    .Returns(Task.CompletedTask);

        // Act
        var result = await _courseController.AddRemoveClasses(Guid.NewGuid(),
            new AddRemoveClassessRequest { AddClassIds = [], RemoveClassIds = [] });

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddRemoveClasses_CourseNotFound_PropagatesNotFoundException()
    {
        // Arrange
        _mockCourseService.Setup(s => s.AddRemoveClassesInCourseAsync(It.IsAny<Guid>(), It.IsAny<AddRemoveClassessRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseController.AddRemoveClasses(Guid.NewGuid(),
            new AddRemoveClassessRequest { AddClassIds = [], RemoveClassIds = [] }));
    }
}
