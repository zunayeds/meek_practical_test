using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Infrastructure.Repositories;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Repositories;

public class StudentRepositoryTests
{
    private static (AppDbContext dbContext, Mock<ICurrentUser> mockUser, Guid staffUserId) CreateDbContext(Guid? studentId = null)
    {
        var staffUserId = Guid.NewGuid();
        var mockUser = new Mock<ICurrentUser>();

        mockUser.Setup(u => u.UserId).Returns(staffUserId);
        mockUser.Setup(u => u.StudentId).Returns(studentId);

        var dbContext = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            mockUser.Object);

        return (dbContext, mockUser, staffUserId);
    }

    private static StudentRepository CreateStudentRepository(AppDbContext dbContext, ICurrentUser user)
        => new(dbContext, new Mock<IMappingHelper>().Object, user);

    private static async Task SeedStaffAsync(AppDbContext dbContext, Guid staffUserId)
    {
        dbContext.Users.Add(new ApplicationUser { Id = staffUserId, FirstName = "Admin", LastName = "Staff", UserName = "admin@live.com", Email = "admin@live.com" });
        await dbContext.SaveChangesAsync();
    }

    private static async Task<Student> SeedStudentAsync(AppDbContext dbContext, string first = "John", string last = "Doe")
    {
        var s = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = first,
            LastName = last,
            EmailAddress = $"{first.ToLower()}@live.com",
            PhoneNumber = "1234567890",
            Address = "Address",
            UserId = Guid.NewGuid()
        };
        dbContext.Students.Add(s);
        await dbContext.SaveChangesAsync();
        return s;
    }

    private static async Task<Class> SeedClassAsync(AppDbContext dbContext, string name = "Math 101")
    {
        var @class = new Class { ClassId = Guid.NewGuid(), Name = name };
        dbContext.Classes.Add(@class);
        await dbContext.SaveChangesAsync();
        return @class;
    }

    private static async Task<Course> SeedCoursesAsync(AppDbContext dbContext, string name = "Programming")
    {
        var course = new Course { CourseId = Guid.NewGuid(), Name = name };
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync();
        return course;
    }

    #region GetOtherStudentNamesByClassIdAsync

    [Fact]
    public async Task GetOtherStudentNames_NoStudentIdInContext_ThrowsNotFoundException()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            studentRepository.GetOtherStudentNamesByClassIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetOtherStudentNames_ExcludesCurrentStudent_ReturnsOthersInFullNameFormat()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);

        var johnny = await SeedStudentAsync(dbContext, "Johnny", "Bravo");
        var bob = await SeedStudentAsync(dbContext, "Bob", "Marley");
        var @class = await SeedClassAsync(dbContext);
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        dbContext.StudentClasses.AddRange(
            new StudentClass { StudentId = johnny.StudentId, ClassId = @class.ClassId },
            new StudentClass { StudentId = bob.StudentId, ClassId = @class.ClassId });
        await dbContext.SaveChangesAsync();

        mockUser.Setup(u => u.StudentId).Returns(johnny.StudentId);

        // Act
        var result = await studentRepository.GetOtherStudentNamesByClassIdAsync(@class.ClassId);

        // Assert
        Assert.Single(result);
        Assert.Equal("Bob Marley", result[0]);
    }

    [Fact]
    public async Task GetOtherStudentNames_NoOtherStudents_ReturnsEmpty()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        var john = await SeedStudentAsync(dbContext);
        var @class = await SeedClassAsync(dbContext);
        dbContext.StudentClasses.Add(new StudentClass { StudentId = john.StudentId, ClassId = @class.ClassId });
        await dbContext.SaveChangesAsync();

        mockUser.Setup(u => u.StudentId).Returns(john.StudentId);

        // Act
        var result = await studentRepository.GetOtherStudentNamesByClassIdAsync(@class.ClassId);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region GetClassesAsync

    [Fact]
    public async Task GetClasses_NoEnrolments_ReturnsEmptyList()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        // Act
        var student = await SeedStudentAsync(dbContext);

        // Assert
        Assert.Empty(await studentRepository.GetClassesAsync(student.StudentId));
    }

    [Fact]
    public async Task GetClasses_ReturnsCorrectDataAndAssignedByFullName()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);
        var student = await SeedStudentAsync(dbContext);
        var @class = await SeedClassAsync(dbContext, "Physics 101");
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        dbContext.StudentClasses.Add(new StudentClass { StudentId = student.StudentId, ClassId = @class.ClassId });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await studentRepository.GetClassesAsync(student.StudentId);

        // Assert
        Assert.Single(result);
        Assert.Equal(@class.ClassId, result[0].ClassId);
        Assert.Equal("Physics 101", result[0].Name);
        Assert.Equal("Admin Staff", result[0].AssignedBy);
        Assert.True(result[0].AssignedAt > DateTimeOffset.MinValue);
    }

    #endregion

    #region GetCoursesAsync

    [Fact]
    public async Task GetCourses_NoEnrolments_ReturnsEmptyList()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        // Act
        var student = await SeedStudentAsync(dbContext);

        // Assert
        Assert.Empty(await studentRepository.GetCoursesAsync(student.StudentId));
    }

    [Fact]
    public async Task GetCourses_ReturnsCorrectDataAndAssignedByFullName()
    {
        // Arrange
        var (dbContext, mockUser, staffUserId) = CreateDbContext();
        await SeedStaffAsync(dbContext, staffUserId);
        var student = await SeedStudentAsync(dbContext);
        var course = await SeedCoursesAsync(dbContext, "Management");
        var studentRepository = CreateStudentRepository(dbContext, mockUser.Object);

        dbContext.StudentCourses.Add(new StudentCourse { StudentId = student.StudentId, CourseId = course.CourseId });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await studentRepository.GetCoursesAsync(student.StudentId);

        // Assert
        Assert.Single(result);
        Assert.Equal(course.CourseId, result[0].CourseId);
        Assert.Equal("Management", result[0].Name);
        Assert.Equal("Admin Staff", result[0].AssignedBy);
        Assert.True(result[0].AssignedAt > DateTimeOffset.MinValue);
    }

    #endregion
}
