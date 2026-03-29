using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Domain.Models.Base;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;
    protected readonly ICurrentUser _currentUser;

    public Repository(AppDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
        _currentUser = currentUser;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync([id], cancellationToken);

    public IQueryable<T> GetAll() => _dbSet.AsQueryable();

    public IQueryable<T> GetByFilters(Expression<Func<T, bool>>? filter = null, int? page = 0, int? pageSize = null)
    {
        var query = _dbSet.AsQueryable();
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        if (pageSize is not null && page > 0)
        {
            query = query.Skip(pageSize.Value * (page.Value - 1)).Take(pageSize.Value);
        }

        return query;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity is IAuditTrailBase auditTrailBase)
        {
            auditTrailBase.CreatedAt = DateTimeOffset.UtcNow;
            auditTrailBase.CreatedBy = _currentUser.UserId;
        }
        else if (entity is AssignmentBase assignmentBase)
        {
            assignmentBase.AssignedAt = DateTimeOffset.UtcNow;
            assignmentBase.AssignedBy = _currentUser.UserId;
        }

        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        if (entity is IAuditTrailBase auditTrailBase)
        {
            auditTrailBase.ModifiedAt = DateTimeOffset.UtcNow;
            auditTrailBase.ModifiedBy = _currentUser.UserId;
        }
        _dbSet.Update(entity);
    }

    public void Remove(T entity) => _dbSet.Remove(entity);
}
