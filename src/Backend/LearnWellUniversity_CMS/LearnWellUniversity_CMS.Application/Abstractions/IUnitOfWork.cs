using LearnWellUniversity_CMS.Application.Repositories;

namespace LearnWellUniversity_CMS.Application.Abstractions;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IClassRepository Classes { get; }
    ICourseRepository Courses { get; }
    IStudentRepository Students { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}