using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext dbContext, ICurrentUser currentUser) : IUnitOfWork
{
    public IClassRepository Classes => new ClassRepository(dbContext, currentUser);
    public ICourseRepository Courses => new CourseRepository(dbContext, currentUser);
    public IStudentRepository Students => new StudentRepository(dbContext, currentUser);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public void Dispose() => dbContext.Dispose();

    public async ValueTask DisposeAsync() => await dbContext.DisposeAsync();
}
