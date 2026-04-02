using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Services;

public class ClassServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IClassRepository> _mockClassRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IMappingHelper> _mockMappingHelper;
    private readonly ClassService _classService;

    public ClassServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockClassRepository = new Mock<IClassRepository>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockMappingHelper = new Mock<IMappingHelper>();

        _mockUnitOfWork.Setup(u => u.Classes).Returns(_mockClassRepository.Object);
        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepository.Object);

        _classService = new ClassService(_mockUnitOfWork.Object, _mockCurrentUser.Object, _mockMappingHelper.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_DuplicateName_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() =>
            _classService.CreateAsync(new CreateClassRequest { Name = "Existing" }));

        _mockClassRepository.Verify(r => r.AddAsync(It.IsAny<Class>(), default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UniqueName_AddsAndReturnsResponse()
    {
        // Arrange
        var expectedResponse = new CreatedEntityResponse { Id = Guid.NewGuid() };

        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(false);
        _mockClassRepository.Setup(r => r.AddAsync(It.IsAny<Class>(), default)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockMappingHelper.Setup(m => m.MapTo<CreatedEntityResponse>(It.IsAny<Class>())).Returns(expectedResponse);

        // Act
        var result = await _classService.CreateAsync(new CreateClassRequest { Name = "New Class" });

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockClassRepository.Verify(r => r.AddAsync(It.IsAny<Class>(), default), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region GetByClassIdAsync

    [Fact]
    public async Task GetByClassIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Class?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _classService.GetByClassIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByClassIdAsync_Found_ReturnsMappedResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var @class = new Class { ClassId = id, Name = "Math 101" };
        var response = new ClassResponse { ClassId = id, Name = "Math 101" };

        _mockClassRepository.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(@class);
        _mockMappingHelper.Setup(m => m.MapTo<ClassResponse>(@class)).Returns(response);

        // Act
        var result = await _classService.GetByClassIdAsync(id);

        // Assert
        Assert.Equal(response, result);
    }

    #endregion

    #region GetClasssAsync

    [Fact]
    public async Task GetClasssAsync_StaffUser_DoesNotFilterByStudentId()
    {
        // Arrange
        _mockCurrentUser.Setup(u => u.StudentId).Returns((Guid?)null);
        var expected = new List<ClassResponseBase> { new() { Name = "Math" } };
        _mockClassRepository.Setup(r => r.GetByFiltersAsync<ClassResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _classService.GetClasssAsync(new GetByFiltersBaseRequest());

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetClasssAsync_StudentUser_FiltersByStudentId()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _mockCurrentUser.Setup(u => u.StudentId).Returns(studentId);
        var expected = new List<ClassResponseBase> { new() { Name = "Science" } };
        _mockClassRepository.Setup(r => r.GetByFiltersAsync<ClassResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _classService.GetClasssAsync(new GetByFiltersBaseRequest());

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region UpdateClassAsync

    [Fact]
    public async Task UpdateClassAsync_ClassNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _classService.UpdateClassAsync(Guid.NewGuid(), new CreateUpdateClassRequest { Name = "New" }));
    }

    [Fact]
    public async Task UpdateClassAsync_DuplicateName_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockClassRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true)   // class exists
                      .ReturnsAsync(true);  // duplicate name

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() =>
            _classService.UpdateClassAsync(Guid.NewGuid(), new CreateUpdateClassRequest { Name = "Duplicate" }));
    }

    [Fact]
    public async Task UpdateClassAsync_ValidRequest_UpdatesAndReturnsResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateUpdateClassRequest { Name = "Updated Name" };
        var updatedClass = new Class { ClassId = id, Name = "Updated Name" };
        var response = new ClassResponse { ClassId = id, Name = "Updated Name" };

        _mockClassRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true)   // class exists
                      .ReturnsAsync(false); // no duplicate name

        _mockClassRepository.Setup(r => r.Update(id, request, default)).ReturnsAsync(updatedClass);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockMappingHelper.Setup(m => m.MapTo<ClassResponse>(updatedClass)).Returns(response);

        // Act
        var result = await _classService.UpdateClassAsync(id, request);

        // Assert
        Assert.Equal(response, result);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region DeleteClassByIdAsync

    [Fact]
    public async Task DeleteClassByIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _classService.DeleteClassByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteClassByIdAsync_Found_Deletes()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true);
        _mockClassRepository.Setup(r => r.DeleteByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        await _classService.DeleteClassByIdAsync(Guid.NewGuid());

        // Assert
        _mockClassRepository.Verify(r => r.DeleteByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region AddRemoveStudentsInClassAsync

    [Fact]
    public async Task AddRemoveStudentsInClassAsync_ClassNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(false);

        // Act
        var request = new AddRemoveStudentsRequest { AddStudentIds = [], RemoveStudentIds = [] };

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _classService.AddRemoveStudentsInClassAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task AddRemoveStudentsInClassAsync_ExecutesInTransactionOrder()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var request = new AddRemoveStudentsRequest
        {
            AddStudentIds = [Guid.NewGuid()],
            RemoveStudentIds = []
        };

        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true);

        var callOrder = new List<string>();

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default))
                .Callback(() => callOrder.Add("BeginTransaction"))
                .Returns(Task.CompletedTask);

        _mockClassRepository.Setup(r => r.AddRemoveStudentsAsync(classId, request.AddStudentIds, request.RemoveStudentIds, default))
                      .Callback(() => callOrder.Add("AddRemoveStudents"))
                      .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .Callback(() => callOrder.Add("SaveChanges"))
                .ReturnsAsync(1);

        _mockUnitOfWork.Setup(u => u.CommitAsync(default))
                .Callback(() => callOrder.Add("Commit"))
                .Returns(Task.CompletedTask);

        // Act
        await _classService.AddRemoveStudentsInClassAsync(classId, request);

        // Assert
        Assert.Equal(["BeginTransaction", "AddRemoveStudents", "SaveChanges", "Commit"], callOrder);
    }

    #endregion

    #region GetStudentsInClassAsync

    [Fact]
    public async Task GetStudentsInClassAsync_ClassNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _classService.GetStudentsInClassAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetStudentsInClassAsync_ReturnsStudentList()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var students = new List<StudentResponseBase>
        {
            new() { StudentId = Guid.NewGuid(), FirstName = "Alice" }
        };

        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true);
        _mockStudentRepository.Setup(r => r.GetByFiltersAsync<StudentResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(students);

        // Act
        var result = await _classService.GetStudentsInClassAsync(classId);

        // Assert
        Assert.Equal(students, result);
    }

    #endregion

    #region GetCoursesAssociatedWithClassAsync

    [Fact]
    public async Task GetCoursesAssociatedWithClassAsync_ClassNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _classService.GetCoursesAssociatedWithClassAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetCoursesAssociatedWithClassAsync_ReturnsCourseList()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var courses = new List<CourseResponseBase>
        {
            new() { CourseId = Guid.NewGuid(), Name = "Physics" }
        };

        _mockClassRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), default))
                      .ReturnsAsync(true);
        _mockCourseRepository.Setup(r => r.GetByFiltersAsync<CourseResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(courses);

        // Act
        var result = await _classService.GetCoursesAssociatedWithClassAsync(classId);

        // Assert
        Assert.Equal(courses, result);
    }

    #endregion
}
