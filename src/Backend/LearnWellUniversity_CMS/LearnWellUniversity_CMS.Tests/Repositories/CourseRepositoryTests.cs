using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Infrastructure.Repositories;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Repositories;

public class CourseRepositoryTests
{
    private static (AppDbContext dbContext, Guid staffUserId) CreateDbContext()
    {
        var staffUserId = Guid.NewGuid();
        var mockUser = new Mock<ICurrentUser>();

        mockUser.Setup(u => u.UserId).Returns(staffUserId);
        mockUser.Setup(u => u.StudentId).Returns((Guid?)null);

        var dbContext = new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            mockUser.Object);

        return (dbContext, staffUserId);
    }

    private static CourseRepository CreateCourseRepository(AppDbContext dbContext)
        => new(dbContext, new Mock<IMappingHelper>().Object, new Mock<ILogger<CourseRepository>>().Object);

    private static async Task<(Course course, Class @class, List<Student> students)> SeedAsync
        (AppDbContext dbContext, Guid staffUserId, int studentCount = 1, bool assignClassToCourse = true)
    {
        dbContext.Users.Add(new ApplicationUser { Id = staffUserId, FirstName = "Admin", LastName = "Staff", UserName = "admin@hotmail.com", Email = "admin@hotmail.com" });

        var course = new Course { CourseId = Guid.NewGuid(), Name = "Applied Physics" };
        dbContext.Courses.Add(course);

        var @class = new Class { ClassId = Guid.NewGuid(), Name = "Phy 101" };
        dbContext.Classes.Add(@class);

        var students = Enumerable.Range(1, studentCount).Select(i => new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = $"Student{i}",
            LastName = "Smith",
            EmailAddress = $"student{i}@gmail.com",
            PhoneNumber = $"{i:D10}",
            Address = "Address",
            UserId = Guid.NewGuid()
        }).ToList();
        dbContext.Students.AddRange(students);

        await dbContext.SaveChangesAsync();

        if (assignClassToCourse)
        {
            dbContext.CourseClasses.Add(new CourseClass { CourseId = course.CourseId, ClassId = @class.ClassId });
            await dbContext.SaveChangesAsync();
        }
        return (course, @class, students);
    }

    #region AddRemoveStudentsAsync

    [Fact]
    public async Task AddRemoveStudents_BothListsEmpty_NoChanges()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, _, _) = await SeedAsync(dbContext, staffUserId);

        // Act
        await CreateCourseRepository(dbContext).AddRemoveStudentsAsync(course.CourseId, [], []);

        // Assert
        Assert.Empty(await dbContext.StudentCourses.ToListAsync());
    }

    [Fact]
    public async Task AddRemoveStudents_ValidStudent_CreatesBothStudentCourseAndStudentClass()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, @class, students) = await SeedAsync(dbContext, staffUserId, assignClassToCourse: true);

        // Act
        await CreateCourseRepository(dbContext).AddRemoveStudentsAsync(course.CourseId, [students[0].StudentId], []);
        await dbContext.SaveChangesAsync();

        // Act & Assert
        Assert.Single(await dbContext.StudentCourses.ToListAsync());
        var sc = await dbContext.StudentClasses.SingleAsync();
        Assert.Equal(@class.ClassId, sc.ClassId);
        Assert.Equal(students[0].StudentId, sc.StudentId);
    }

    [Fact]
    public async Task AddRemoveStudents_AlreadyEnrolled_SkipsExisting()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, _, students) = await SeedAsync(dbContext, staffUserId);
        var repo = CreateCourseRepository(dbContext);
        var id = students[0].StudentId;

        // Act
        await repo.AddRemoveStudentsAsync(course.CourseId, [id], []);
        await dbContext.SaveChangesAsync();

        // Act & Assert
        await repo.AddRemoveStudentsAsync(course.CourseId, [id], []);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Single(await dbContext.StudentCourses.ToListAsync());
    }

    [Fact]
    public async Task AddRemoveStudents_InvalidStudentId_ThrowsNotFoundException()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, @class, students) = await SeedAsync(dbContext, staffUserId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateCourseRepository(dbContext).AddRemoveStudentsAsync(course.CourseId, [Guid.NewGuid()], []));
    }

    #endregion

    #region AddRemoveClassesAsync
    
    [Fact]
    public async Task AddClasses_BothListsEmpty_NoChanges()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, _, _) = await SeedAsync(dbContext, staffUserId, assignClassToCourse: false);

        // Act
        await CreateCourseRepository(dbContext).AddRemoveClassesAsync(course.CourseId, [], []);

        // Assert
        Assert.Empty(await dbContext.CourseClasses.ToListAsync());
    }

    [Fact]
    public async Task AddClasses_ValidClass_CreatesCourseClassRecord()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, @class, _) = await SeedAsync(dbContext, staffUserId, assignClassToCourse: false);

        // Act
        await CreateCourseRepository(dbContext).AddRemoveClassesAsync(course.CourseId, [@class.ClassId], []);
        await dbContext.SaveChangesAsync();

        // Assert
        var cc = await dbContext.CourseClasses.SingleAsync();
        Assert.Equal(course.CourseId, cc.CourseId);
        Assert.Equal(@class.ClassId, cc.ClassId);
    }

    [Fact]
    public async Task AddClasses_WithEnrolledStudents_CreatesStudentClassForNewClass()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, @class, students) = await SeedAsync(dbContext, staffUserId, assignClassToCourse: false);

        dbContext.StudentCourses.Add(new StudentCourse { StudentId = students[0].StudentId, CourseId = course.CourseId });
        await dbContext.SaveChangesAsync();

        var newClass = new Class { ClassId = Guid.NewGuid(), Name = "Math 101" };
        dbContext.Classes.Add(newClass);
        await dbContext.SaveChangesAsync();

        // Act
        await CreateCourseRepository(dbContext).AddRemoveClassesAsync(course.CourseId, [newClass.ClassId], []);
        await dbContext.SaveChangesAsync();

        // Assert
        var studentClasses = await dbContext.StudentClasses.Where(sc => sc.ClassId == newClass.ClassId).ToListAsync();
        Assert.Single(studentClasses);
        Assert.Equal(students[0].StudentId, studentClasses[0].StudentId);
    }

    [Fact]
    public async Task AddClasses_InvalidClassId_ThrowsNotFoundException()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (course, @class, students) = await SeedAsync(dbContext, staffUserId, assignClassToCourse: false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateCourseRepository(dbContext).AddRemoveClassesAsync(course.CourseId, [Guid.NewGuid()], []));
    }

    #endregion
}
