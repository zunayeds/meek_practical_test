using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Services;

public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IClassRepository> _mockClassRepository;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IMappingHelper> _mockMappingHelper;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockClassRepository = new Mock<IClassRepository>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockMappingHelper = new Mock<IMappingHelper>();

        _mockUnitOfWork.Setup(u => u.Courses).Returns(_mockCourseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Classes).Returns(_mockClassRepository.Object);
        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepository.Object);

        _courseService = new CourseService(_mockUnitOfWork.Object, _mockCurrentUser.Object, _mockMappingHelper.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_DuplicateName_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() =>
            _courseService.CreateAsync(new CreateUpdateCourseRequest { Name = "Existing" }));

        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>(), default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UniqueName_AddsAndReturnsResponse()
    {
        // Arrange
        var expectedResponse = new CreatedEntityResponse { Id = Guid.NewGuid() };

        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);
        _mockCourseRepository.Setup(r => r.AddAsync(It.IsAny<Course>(), default)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockMappingHelper.Setup(m => m.MapTo<CreatedEntityResponse>(It.IsAny<Course>())).Returns(expectedResponse);

        // Act
        var result = await _courseService.CreateAsync(new CreateUpdateCourseRequest { Name = "New Course" });

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockCourseRepository.Verify(r => r.AddAsync(It.IsAny<Course>(), default), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region GetByCourseIdAsync

    [Fact]
    public async Task GetByCourseIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Course?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.GetByCourseIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByCourseIdAsync_Found_ReturnsMappedResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var course = new Course { CourseId = id, Name = "Physics 101" };
        var response = new CourseResponse { CourseId = id, Name = "Physics 101" };

        _mockCourseRepository.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(course);
        _mockMappingHelper.Setup(m => m.MapTo<CourseResponse>(course)).Returns(response);

        // Act
        var result = await _courseService.GetByCourseIdAsync(id);

        // Assert
        Assert.Equal(response, result);
    }

    #endregion

    #region GetCoursesAsync

    [Fact]
    public async Task GetCoursesAsync_StaffUser_ReturnsAllCourses()
    {
        // Arrange
        _mockCurrentUser.Setup(u => u.StudentId).Returns((Guid?)null);
        var expected = new List<CourseResponseBase> { new() { Name = "Physics" } };
        _mockCourseRepository.Setup(r => r.GetByFiltersAsync<CourseResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _courseService.GetCoursesAsync(new GetByFiltersBaseRequest());

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetCoursesAsync_StudentUser_FiltersByStudentId()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _mockCurrentUser.Setup(u => u.StudentId).Returns(studentId);
        var expected = new List<CourseResponseBase> { new() { Name = "Math" } };
        _mockCourseRepository.Setup(r => r.GetByFiltersAsync<CourseResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _courseService.GetCoursesAsync(new GetByFiltersBaseRequest());

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region UpdateCourseAsync

    [Fact]
    public async Task UpdateCourseAsync_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _courseService.UpdateCourseAsync(Guid.NewGuid(), new CreateUpdateCourseRequest { Name = "New" }));
    }

    [Fact]
    public async Task UpdateCourseAsync_DuplicateName_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockCourseRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true)   // course exists
                       .ReturnsAsync(true);  // duplicate name

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() =>
            _courseService.UpdateCourseAsync(Guid.NewGuid(), new CreateUpdateCourseRequest { Name = "Duplicate" }));
    }

    [Fact]
    public async Task UpdateCourseAsync_ValidRequest_UpdatesAndReturnsResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateUpdateCourseRequest { Name = "Updated Name" };
        var updatedCourse = new Course { CourseId = id, Name = "Updated Name" };
        var response = new CourseResponse { CourseId = id, Name = "Updated Name" };

        _mockCourseRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true)   // course exists
                       .ReturnsAsync(false); // no duplicate name

        _mockCourseRepository.Setup(r => r.Update(id, request, default)).ReturnsAsync(updatedCourse);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockMappingHelper.Setup(m => m.MapTo<CourseResponse>(updatedCourse)).Returns(response);

        // Act
        var result = await _courseService.UpdateCourseAsync(id, request);

        // Assert
        Assert.Equal(response, result);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region DeleteCourseByIdAsync

    [Fact]
    public async Task DeleteCourseByIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.DeleteCourseByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteCourseByIdAsync_Found_Deletes()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true);
        _mockCourseRepository.Setup(r => r.DeleteByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        await _courseService.DeleteCourseByIdAsync(Guid.NewGuid());

        // Assert
        _mockCourseRepository.Verify(r => r.DeleteByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region AddRemoveStudentsInCourseAsync

    [Fact]
    public async Task AddRemoveStudentsInCourseAsync_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);

        var request = new AddRemoveStudentsRequest { AddStudentIds = [], RemoveStudentIds = [] };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _courseService.AddRemoveStudentsInCourseAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task AddRemoveStudentsInCourseAsync_ExecutesInTransactionOrder()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new AddRemoveStudentsRequest { AddStudentIds = [Guid.NewGuid()], RemoveStudentIds = [] };

        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true);

        var callOrder = new List<string>();

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default))
                .Callback(() => callOrder.Add("BeginTransaction"))
                .Returns(Task.CompletedTask);

        _mockCourseRepository.Setup(r => r.AddRemoveStudentsAsync(courseId, request.AddStudentIds, request.RemoveStudentIds, default))
                       .Callback(() => callOrder.Add("AddRemoveStudents"))
                       .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .Callback(() => callOrder.Add("SaveChanges"))
                .ReturnsAsync(1);

        _mockUnitOfWork.Setup(u => u.CommitAsync(default))
                .Callback(() => callOrder.Add("Commit"))
                .Returns(Task.CompletedTask);

        // Act
        await _courseService.AddRemoveStudentsInCourseAsync(courseId, request);

        // Assert
        Assert.Equal(["BeginTransaction", "AddRemoveStudents", "SaveChanges", "Commit"], callOrder);
    }

    #endregion

    #region GetStudentsInCourseAsync

    [Fact]
    public async Task GetStudentsInCourseAsync_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.GetStudentsInCourseAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetStudentsInCourseAsync_ReturnsStudentList()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var students = new List<StudentResponseBase> { new() { FirstName = "Alice" } };

        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true);
        _mockStudentRepository.Setup(r => r.GetByFiltersAsync<StudentResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(students);

        // Act
        var result = await _courseService.GetStudentsInCourseAsync(courseId);

        // Assert
        Assert.Equal(students, result);
    }

    #endregion

    #region GetClassesInCourseAsync

    [Fact]
    public async Task GetClassesInCourseAsync_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.GetClassesInCourseAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetClassesInCourseAsync_ReturnsClassList()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var classes = new List<ClassResponseBase> { new() { Name = "Math 101" } };

        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true);
        _mockClassRepository.Setup(r => r.GetByFiltersAsync<ClassResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Class, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(classes);

        // Act
        var result = await _courseService.GetClassesInCourseAsync(courseId);

        // Assert
        Assert.Equal(classes, result);
    }

    #endregion

    #region AddRemoveClassesInCourseAsync

    [Fact]
    public async Task AddRemoveClassesInCourseAsync_CourseNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(false);

        var request = new AddRemoveClassessRequest { AddClassIds = [], RemoveClassIds = [] };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _courseService.AddRemoveClassesInCourseAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task AddRemoveClassesInCourseAsync_ExecutesInTransactionOrder()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var request = new AddRemoveClassessRequest { AddClassIds = [Guid.NewGuid()], RemoveClassIds = [] };

        _mockCourseRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Course, bool>>>(), default))
                       .ReturnsAsync(true);

        var callOrder = new List<string>();

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default))
                .Callback(() => callOrder.Add("BeginTransaction"))
                .Returns(Task.CompletedTask);

        _mockCourseRepository.Setup(r => r.AddRemoveClassesAsync(courseId, request.AddClassIds, request.RemoveClassIds, default))
                       .Callback(() => callOrder.Add("AddRemoveClasses"))
                       .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .Callback(() => callOrder.Add("SaveChanges"))
                .ReturnsAsync(1);

        _mockUnitOfWork.Setup(u => u.CommitAsync(default))
                .Callback(() => callOrder.Add("Commit"))
                .Returns(Task.CompletedTask);

        // Act
        await _courseService.AddRemoveClassesInCourseAsync(courseId, request);

        // Assert
        Assert.Equal(["BeginTransaction", "AddRemoveClasses", "SaveChanges", "Commit"], callOrder);
    }

    #endregion
}
