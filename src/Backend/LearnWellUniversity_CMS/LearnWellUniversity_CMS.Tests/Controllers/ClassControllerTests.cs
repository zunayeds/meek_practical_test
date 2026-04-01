using LearnWellUniversity_CMS.API.Controllers;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Controllers;

public class ClassControllerTests
{
    private readonly Mock<IClassService> _mockClassService = new();
    private readonly ClassController _classController;

    public ClassControllerTests()
        => _classController = new ClassController(_mockClassService.Object, new Mock<ILogger<ClassController>>().Object);

    [Fact]
    public async Task Create_ValidRequest_ReturnsOk()
    {
        _mockClassService.Setup(s => s.CreateAsync(It.IsAny<CreateClassRequest>(), default))
                    .ReturnsAsync(new CreatedEntityResponse { Id = Guid.NewGuid() });
        Assert.IsType<OkObjectResult>(await _classController.Create(new CreateClassRequest { Name = "Math-101" }));
    }

    [Fact]
    public async Task Create_DuplicateName_PropagatesAlreadyExistException()
    {
        _mockClassService.Setup(s => s.CreateAsync(It.IsAny<CreateClassRequest>(), default))
                    .ThrowsAsync(new AlreadyExistException("exists"));
        await Assert.ThrowsAsync<AlreadyExistException>(() =>
            _classController.Create(new CreateClassRequest { Name = "X" }));
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        _mockClassService.Setup(s => s.GetClasssAsync(It.IsAny<GetByFiltersBaseRequest>(), default))
                    .ReturnsAsync([new ClassResponseBase { Name = "Math-101" }]);
        Assert.IsType<OkObjectResult>(await _classController.GetAll(new GetByFiltersBaseRequest()));
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _mockClassService.Setup(s => s.GetByClassIdAsync(id, default))
                    .ReturnsAsync(new ClassResponse { ClassId = id });
        Assert.IsType<OkObjectResult>(await _classController.GetById(id));
    }

    [Fact]
    public async Task GetById_ClassNotFound_PropagatesNotFoundException()
    {
        _mockClassService.Setup(s => s.GetByClassIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _classController.GetById(Guid.NewGuid()));
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _mockClassService.Setup(s => s.UpdateClassAsync(id, It.IsAny<CreateUpdateClassRequest>(), default))
                    .ReturnsAsync(new ClassResponse { ClassId = id });
        Assert.IsType<OkObjectResult>(await _classController.Update(id, new CreateUpdateClassRequest { Name = "New" }));
    }

    [Fact]
    public async Task Update_ClassNotFound_PropagatesNotFoundException()
    {
        var id = Guid.NewGuid();
        _mockClassService.Setup(s => s.UpdateClassAsync(id, It.IsAny<CreateUpdateClassRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _classController.Update(id, new CreateUpdateClassRequest { Name = "New" }));
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _mockClassService.Setup(s => s.DeleteClassByIdAsync(id, default)).Returns(Task.CompletedTask);
        Assert.IsType<NoContentResult>(await _classController.Delete(id));
    }

    [Fact]
    public async Task Delete_ClassNotFound_PropagatesNotFoundException()
    {
        _mockClassService.Setup(s => s.DeleteClassByIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _classController.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task AddRemoveStudents_ReturnsNoContent()
    {
        _mockClassService.Setup(s => s.AddRemoveStudentsInClassAsync(It.IsAny<Guid>(), It.IsAny<AddRemoveStudentsRequest>(), default))
                    .Returns(Task.CompletedTask);
        var result = await _classController.AddRemoveStudents(Guid.NewGuid(),
            new AddRemoveStudentsRequest { AddStudentIds = [], RemoveStudentIds = [] });
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AddRemoveStudents_ClassNotFound_PropagatesNotFoundException()
    {
        _mockClassService.Setup(s => s.AddRemoveStudentsInClassAsync(It.IsAny<Guid>(), It.IsAny<AddRemoveStudentsRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _classController.AddRemoveStudents(Guid.NewGuid(),
            new AddRemoveStudentsRequest { AddStudentIds = [], RemoveStudentIds = [] }));
    }

    [Fact]
    public async Task GetStudents_ReturnsOkWithList()
    {
        _mockClassService.Setup(s => s.GetStudentsInClassAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new StudentResponseBase { FirstName = "John" }]);
        Assert.IsType<OkObjectResult>(await _classController.GetStudents(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetCourses_ReturnsOkWithList()
    {
        _mockClassService.Setup(s => s.GetCoursesAssociatedWithClassAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new CourseResponseBase { Name = "Physics" }]);
        Assert.IsType<OkObjectResult>(await _classController.GetCourses(Guid.NewGuid()));
    }
}
