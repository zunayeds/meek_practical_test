using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Services;

public class StudentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IStudentRepository> _mockStudentRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly Mock<IMappingHelper> _mockMappingHelper;
    private readonly Mock<ILogger<StudentService>> _mockLogger;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockStudentRepository = new Mock<IStudentRepository>();
        _mockUserService = new Mock<IUserService>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _mockMappingHelper = new Mock<IMappingHelper>();
        _mockLogger = new Mock<ILogger<StudentService>>();

        _mockUnitOfWork.Setup(u => u.Students).Returns(_mockStudentRepository.Object);
        _mockCurrentUser.Setup(u => u.UserId).Returns(Guid.NewGuid());

        _studentService = new StudentService(
            _mockUnitOfWork.Object, _mockUserService.Object,
            _mockCurrentUser.Object, _mockMappingHelper.Object, _mockLogger.Object);
    }

    private CreateUpdateStudentRequest ValidStudentCreateUpdateRequest = new()
    {
        FirstName = "Dave",
        LastName = "Jones",
        EmailAddress = "dave@gmail.com",
        PhoneNumber = "1234567890",
        Address = "Address"
    };

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_DuplicateEmail_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _studentService.CreateAsync(ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task CreateAsync_DuplicatePhoneNumber_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockStudentRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false)  // unique email
                        .ReturnsAsync(true);  // duplicate phone number exists

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _studentService.CreateAsync(ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task CreateAsync_Success_ReturnsResponseWithPassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var generatedPassword = "AutoGen@Pass1";
        var expectedResponse = new CreateStudentResponse { Id = Guid.NewGuid() };

        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _mockUserService.Setup(s => s.CreateWithRoleAsync(It.IsAny<ApplicationUser>(), Roles.Student, null, true))
                        .ReturnsAsync((userId, generatedPassword));
        _mockStudentRepository.Setup(r => r.AddAsync(It.IsAny<Student>(), default)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.CommitAsync(default)).Returns(Task.CompletedTask);
        _mockMappingHelper.Setup(m => m.MapTo<CreateStudentResponse>(It.IsAny<Student>())).Returns(expectedResponse);

        // Act
        var result = await _studentService.CreateAsync(ValidStudentCreateUpdateRequest);

        // Assert
        Assert.Equal(generatedPassword, result.Password);
        _mockUnitOfWork.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UserServiceThrows_RollsBackAndRethrows()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _mockUserService.Setup(s => s.CreateWithRoleAsync(It.IsAny<ApplicationUser>(), Roles.Student, null, true))
                        .ThrowsAsync(new Exception("Unhandled exception"));
        _mockUnitOfWork.Setup(u => u.RollbackAsync(default)).Returns(Task.CompletedTask);

        // Act
        await Assert.ThrowsAsync<Exception>(() => _studentService.CreateAsync(ValidStudentCreateUpdateRequest));

        // Assert
        _mockUnitOfWork.Verify(u => u.RollbackAsync(default), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ExecutesInTransactionOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var callOrder = new List<string>();

        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default))
                .Callback(() => callOrder.Add("BeginTransaction"))
                .Returns(Task.CompletedTask);

        _mockUserService.Setup(s => s.CreateWithRoleAsync(It.IsAny<ApplicationUser>(), Roles.Student, null, true))
                        .Callback(() => callOrder.Add("CreateUser"))
                        .ReturnsAsync((userId, "Pass@1"));

        _mockStudentRepository.Setup(r => r.AddAsync(It.IsAny<Student>(), default))
                        .Callback(() => callOrder.Add("AddStudent"))
                        .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .Callback(() => callOrder.Add("SaveChanges"))
                .ReturnsAsync(1);

        _mockUnitOfWork.Setup(u => u.CommitAsync(default))
                .Callback(() => callOrder.Add("Commit"))
                .Returns(Task.CompletedTask);

        _mockMappingHelper.Setup(m => m.MapTo<CreateStudentResponse>(It.IsAny<Student>()))
                          .Returns(new CreateStudentResponse());

        // Act
        await _studentService.CreateAsync(ValidStudentCreateUpdateRequest);

        // Assert
        Assert.Equal(["BeginTransaction", "CreateUser", "AddStudent", "SaveChanges", "Commit"], callOrder);
    }

    #endregion

    #region GetByStudentIdAsync

    [Fact]
    public async Task GetByStudentIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Student?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.GetByStudentIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByStudentIdAsync_Found_ReturnsMappedResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var student = new Student
        {
            StudentId = id,
            FirstName = "Denise",
            LastName = "Richards",
            EmailAddress = "denise@yahoo.com",
            PhoneNumber = "123456789",
            Address = "Address",
            UserId = Guid.NewGuid()
        };
        var response = new StudentResponse
        {
            StudentId = id,
            FirstName = "Denise",
            LastName = "Richards",
            EmailAddress = "denise@yahoo.com",
            PhoneNumber = "123456789"
        };

        _mockStudentRepository.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(student);
        _mockMappingHelper.Setup(m => m.MapTo<StudentResponse>(student)).Returns(response);

        // Act
        var result = await _studentService.GetByStudentIdAsync(id);

        // Assert
        Assert.Equal(response, result);
    }

    #endregion

    #region GetStudentsAsync

    [Fact]
    public async Task GetStudentsAsync_NoFilter_ReturnsAllStudents()
    {
        // Arrange
        var expected = new PaginatedResponse<StudentResponseBase>
        {
            TotalRecords = 1,
            Records = [new() { FirstName = "Dave" }]
        };
        _mockStudentRepository.Setup(r => r.GetByFiltersAsync<StudentResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _studentService.GetStudentsAsync(new GetStudentByFiltersRequest());

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetStudentsAsync_WithFilters_DelegatesToRepository()
    {
        // Arrange
        var request = new GetStudentByFiltersRequest
        {
            Name = "Dav",
            EmailAddress = "dave",
            PhoneNumber = "123"
        };
        var expected = new PaginatedResponse<StudentResponseBase>
        {
            TotalRecords = 0,
            Records = []
        };
        _mockStudentRepository.Setup(r => r.GetByFiltersAsync<StudentResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _studentService.GetStudentsAsync(request);

        // Assert
        _mockStudentRepository.Verify(r => r.GetByFiltersAsync<StudentResponseBase>(
            It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), It.IsAny<int?>(), It.IsAny<int?>(), default),
            Times.Once);
    }

    #endregion

    #region UpdateStudentAsync

    [Fact]
    public async Task UpdateStudentAsync_StudentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.UpdateStudentAsync(Guid.NewGuid(), ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task UpdateStudentAsync_DuplicateEmail_ThrowsAlreadyExistException()
    {
        // Arrange
        _mockStudentRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true)   // student exists
                        .ReturnsAsync(true);  // duplicate email exists

        // Act & Assert
        await Assert.ThrowsAsync<AlreadyExistException>(() => _studentService.UpdateStudentAsync(Guid.NewGuid(), ValidStudentCreateUpdateRequest));
    }

    [Fact]
    public async Task UpdateStudentAsync_ValidRequest_UpdatesAndReturnsResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new CreateUpdateStudentRequest
        {
            FirstName = "Updated",
            LastName = "Student",
            EmailAddress = "upd@yahoo.com",
            PhoneNumber = "9876",
            Address = "New Addr"
        };
        var updatedStudent = new Student
        {
            StudentId = id,
            FirstName = "Updated",
            LastName = "Student",
            EmailAddress = "upd@yahoo.com",
            PhoneNumber = "9876",
            Address = "New Addr",
            UserId = Guid.NewGuid()
        };
        var response = new StudentResponse { StudentId = id };

        _mockStudentRepository.SetupSequence(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true)   // student exists
                        .ReturnsAsync(false)  // no duplicate email
                        .ReturnsAsync(false); // no duplicate phone

        _mockStudentRepository.Setup(r => r.Update(id, request, default)).ReturnsAsync(updatedStudent);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockMappingHelper.Setup(m => m.MapTo<StudentResponse>(updatedStudent)).Returns(response);

        // Act
        var result = await _studentService.UpdateStudentAsync(id, request);

        // Assert
        Assert.Equal(response, result);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region DeleteStudentByIdAsync

    [Fact]
    public async Task DeleteStudentByIdAsync_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.DeleteStudentByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteStudentByIdAsync_Found_DeletesStudentAndUser()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _mockStudentRepository.Setup(r => r.GetUserIdAsync(studentId, default)).ReturnsAsync(userId);
        _mockStudentRepository.Setup(r => r.DeleteByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .Returns(Task.CompletedTask);
        _mockUserService.Setup(s => s.DeleteUserByIdAsync(userId)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.CommitAsync(default)).Returns(Task.CompletedTask);

        // Act
        await _studentService.DeleteStudentByIdAsync(studentId);

        // Assert
        _mockStudentRepository.Verify(r => r.DeleteByFilterAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default), Times.Once);
        _mockUserService.Verify(s => s.DeleteUserByIdAsync(userId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(default), Times.Once);
    }

    #endregion

    #region GetOtherStudentNamesByClassIdAsync

    [Fact]
    public async Task GetOtherStudentNamesByClassIdAsync_DelegatesToRepository()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var names = new List<string> { "Alice Smith", "Bob Jones" };
        _mockStudentRepository.Setup(r => r.GetOtherStudentNamesByClassIdAsync(classId, default)).ReturnsAsync(names);

        // Act
        var result = await _studentService.GetOtherStudentNamesByClassIdAsync(classId);

        // Assert
        Assert.Equal(names, result);
        _mockStudentRepository.Verify(r => r.GetOtherStudentNamesByClassIdAsync(classId, default), Times.Once);
    }

    #endregion

    #region GetClassesByStudentIdAsync

    [Fact]
    public async Task GetClassesByStudentIdAsync_StudentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.GetClassesByStudentIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetClassesByStudentIdAsync_Found_ReturnsClasses()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classes = new List<StudentClassResponse>
        {
            new() { ClassId = Guid.NewGuid(), Name = "Math 101" }
        };

        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true);
        _mockStudentRepository.Setup(r => r.GetClassesAsync(studentId, default)).ReturnsAsync(classes);

        // Act
        var result = await _studentService.GetClassesByStudentIdAsync(studentId);

        // Assert
        Assert.Equal(classes, result);
    }

    #endregion

    #region GetClassesAsync (current student)

    [Fact]
    public async Task GetClassesAsync_NoStudentIdInContext_ThrowsNotFoundException()
    {
        // Arrange
        _mockCurrentUser.Setup(u => u.StudentId).Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.GetClassesAsync());
    }

    [Fact]
    public async Task GetClassesAsync_StudentInContext_ReturnsClasses()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classes = new List<StudentClassResponse>
        {
            new() { ClassId = Guid.NewGuid(), Name = "Science 101" }
        };

        _mockCurrentUser.Setup(u => u.StudentId).Returns(studentId);
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true);
        _mockStudentRepository.Setup(r => r.GetClassesAsync(studentId, default)).ReturnsAsync(classes);

        // Act
        var result = await _studentService.GetClassesAsync();

        // Assert
        Assert.Equal(classes, result);
    }

    #endregion

    #region GetCoursesByStudentIdAsync

    [Fact]
    public async Task GetCoursesByStudentIdAsync_StudentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.GetCoursesByStudentIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetCoursesByStudentIdAsync_Found_ReturnsCourses()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classes = new List<StudentCourseResponse>
        {
            new() { CourseId = Guid.NewGuid(), Name = "Programming" }
        };

        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true);
        _mockStudentRepository.Setup(r => r.GetCoursesAsync(studentId, default)).ReturnsAsync(classes);

        // Act
        var result = await _studentService.GetCoursesByStudentIdAsync(studentId);

        // Assert
        Assert.Equal(classes, result);
    }

    #endregion

    #region GetCoursesAsync (current student)

    [Fact]
    public async Task GetCoursesAsync_NoStudentIdInContext_ThrowsNotFoundException()
    {
        // Arrange
        _mockCurrentUser.Setup(u => u.StudentId).Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _studentService.GetCoursesAsync());
    }

    [Fact]
    public async Task GetCoursesAsync_StudentInContext_ReturnsCourses()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var classes = new List<StudentCourseResponse>
        {
            new() { CourseId = Guid.NewGuid(), Name = "Management" }
        };

        _mockCurrentUser.Setup(u => u.StudentId).Returns(studentId);
        _mockStudentRepository.Setup(r => r.DoesExistAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Student, bool>>>(), default))
                        .ReturnsAsync(true);
        _mockStudentRepository.Setup(r => r.GetCoursesAsync(studentId, default)).ReturnsAsync(classes);

        // Act
        var result = await _studentService.GetCoursesAsync();

        // Assert
        Assert.Equal(classes, result);
    }

    #endregion
}
