using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using LearnWellUniversity_CMS.Infrastructure.Repositories;
using LearnWellUniversity_CMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace LearnWellUniversity_CMS.Tests.Repositories;

public class ClassRepositoryTests
{
    private static (AppDbContext dbContextContext, Guid staffUserId) CreateDbContext()
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

    private static ClassRepository CreateClassRepository(AppDbContext dbContext)
        => new(dbContext, new Mock<IMappingHelper>().Object, new Mock<ILogger<ClassRepository>>().Object);

    private static async Task<(Class, List<Student>)> SeedAsync(AppDbContext dbContext, Guid staffUserId, int studentCount = 2)
    {
        dbContext.Users.Add(new ApplicationUser { Id = staffUserId, FirstName = "Admin", LastName = "Staff", UserName = "admin@outlook.com", Email = "admin@outlook.com" });

        var @class = new Class { ClassId = Guid.NewGuid(), Name = "Math 101" };
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

        return (@class, students);
    }

    #region AddRemoveStudentsAsync

    [Fact]
    public async Task AddRemoveStudents_BothListsEmpty_NoChanges()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (@class, _) = await SeedAsync(dbContext, staffUserId);
        var classRepository = CreateClassRepository(dbContext);

        // Act
        await classRepository.AddRemoveStudentsAsync(@class.ClassId, [], []);

        // Assert
        Assert.Empty(await dbContext.StudentClasses.ToListAsync());
    }

    [Fact]
    public async Task AddRemoveStudents_ValidStudents_CreatesStudentClassRecords()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (@class, students) = await SeedAsync(dbContext, staffUserId);
        var classRepository = CreateClassRepository(dbContext);

        // Act
        await classRepository.AddRemoveStudentsAsync(@class.ClassId, [.. students.Select(s => s.StudentId)], []);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(students.Count, await dbContext.StudentClasses.CountAsync());
    }

    [Fact]
    public async Task AddRemoveStudents_AlreadyEnrolled_SkipsExisting()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (@class, students) = await SeedAsync(dbContext, staffUserId, 1);
        var classRepository = CreateClassRepository(dbContext);
        var id = students[0].StudentId;

        // Act
        await classRepository.AddRemoveStudentsAsync(@class.ClassId, [id], []);
        await dbContext.SaveChangesAsync();

        await classRepository.AddRemoveStudentsAsync(@class.ClassId, [id], []);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Single(await dbContext.StudentClasses.ToListAsync());
    }

    [Fact]
    public async Task AddRemoveStudents_InvalidStudentId_ThrowsNotFoundException()
    {
        // Arrange
        var (dbContext, staffUserId) = CreateDbContext();
        var (@class, _) = await SeedAsync(dbContext, staffUserId);
        var classRepository = CreateClassRepository(dbContext);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            classRepository.AddRemoveStudentsAsync(@class.ClassId, [Guid.NewGuid()], []));
    } 

    #endregion
}
