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

    private CreateUpdateStudentRequest ValidStudentCreateUpdateRequest = new()
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
        _mockStudentService.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ReturnsAsync(new CreateStudentResponse { Id = Guid.NewGuid(), Password = "Generated@Pass1" });
        var result = await _studentController.Create(ValidStudentCreateUpdateRequest);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotEmpty(((CreateStudentResponse)ok.Value!).Password);
    }

    [Fact]
    public async Task Create_DuplicateEmailOrPhone_PropagatesAlreadyExistException()
    {
        _mockStudentService.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ThrowsAsync(new AlreadyExistException("exists"));
        await Assert.ThrowsAsync<AlreadyExistException>(() => _studentController.Create(ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        _mockStudentService.Setup(s => s.GetStudentsAsync(It.IsAny<GetStudentByFiltersRequest>(), default))
                    .ReturnsAsync([new StudentResponseBase { FirstName = "Alice" }]);
        Assert.IsType<OkObjectResult>(await _studentController.GetAll(new GetStudentByFiltersRequest()));
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _mockStudentService.Setup(s => s.GetByStudentIdAsync(id, default)).ReturnsAsync(new StudentResponse { StudentId = id });
        Assert.IsType<OkObjectResult>(await _studentController.GetById(id));
    }

    [Fact]
    public async Task GetById_StudentNotFound_PropagatesNotFoundException()
    {
        _mockStudentService.Setup(s => s.GetByStudentIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _studentController.GetById(Guid.NewGuid()));
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _mockStudentService.Setup(s => s.UpdateStudentAsync(id, It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ReturnsAsync(new StudentResponse { StudentId = id });
        Assert.IsType<OkObjectResult>(await _studentController.Update(id, ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task Update_StudentNotFound_PropagatesNotFoundException()
    {
        var id = Guid.NewGuid();
        _mockStudentService.Setup(s => s.UpdateStudentAsync(id, It.IsAny<CreateUpdateStudentRequest>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _studentController.Update(id, ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        _mockStudentService.Setup(s => s.DeleteStudentByIdAsync(It.IsAny<Guid>(), default)).Returns(Task.CompletedTask);
        Assert.IsType<NoContentResult>(await _studentController.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task Delete_StudentNotFound_PropagatesNotFoundException()
    {
        _mockStudentService.Setup(s => s.DeleteStudentByIdAsync(It.IsAny<Guid>(), default))
                    .ThrowsAsync(new NotFoundException("not found"));
        await Assert.ThrowsAsync<NotFoundException>(() => _studentController.Delete(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetOtherStudents_ReturnsOkWithNameList()
    {
        _mockStudentService.Setup(s => s.GetOtherStudentNamesByClassIdAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync(["Alice Smith"]);
        Assert.IsType<OkObjectResult>(await _studentController.GetOtherStudents(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetClassesByStudentId_ReturnsOk()
    {
        _mockStudentService.Setup(s => s.GetClassesByStudentIdAsync(It.IsAny<Guid>(), default))
                    .ReturnsAsync([new StudentClassResponse { Name = "Math-101" }]);
        Assert.IsType<OkObjectResult>(await _studentController.GetClassesByStudentId(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetClasses_ReturnsOk()
    {
        _mockStudentService.Setup(s => s.GetClassesAsync(default))
                    .ReturnsAsync([new StudentClassResponse { Name = "Science-101" }]);
        Assert.IsType<OkObjectResult>(await _studentController.GetClasses());
    }
}
