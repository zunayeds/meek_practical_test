using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Repositories;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext dbContext, IClassRepository classes, ICourseRepository courses, IStudentRepository students) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public IClassRepository Classes => classes;
    public ICourseRepository Courses => courses;
    public IStudentRepository Students => students;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
        }
        await dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
