using LearnWellUniversity_CMS.Application.Repositories;

namespace LearnWellUniversity_CMS.Application.Abstractions;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IClassRepository Classes { get; }
    ICourseRepository Courses { get; }
    IStudentRepository Students { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}