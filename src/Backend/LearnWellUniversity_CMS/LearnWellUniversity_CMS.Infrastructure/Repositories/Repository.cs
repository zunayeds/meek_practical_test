using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core;
using System.Linq.Expressions;

namespace LearnWellUniversity_CMS.Infrastructure.Repositories;

public class Repository<T>(AppDbContext dbContext, IMappingHelper mappingHelper) : IRepository<T> where T : class
{
    protected readonly AppDbContext _dbContext = dbContext;
    protected readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync([id], cancellationToken);

    public async Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) =>
        await _dbSet.FirstOrDefaultAsync(filter, cancellationToken);

    public IQueryable<T> GetAllAsQueryable() => _dbSet.AsQueryable();

    public IQueryable<T> GetByFiltersAsQueryable(Expression<Func<T, bool>>? filter = null, int? page = 0, int? pageSize = null)
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

    public async Task<List<TResponseType>> GetByFiltersAsync<TResponseType>(Expression<Func<T, bool>>? filter = null, int? page = 0, int? pageSize = null, CancellationToken cancellationToken = default) where TResponseType : class
    {
        var query = GetByFiltersAsQueryable(filter, page, pageSize);

        return await mappingHelper.ProjectTo<TResponseType>(query).ToListAsync(cancellationToken); ;
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<T> Update<TUpdate>(Guid Id, TUpdate update, CancellationToken cancellationToken = default) where TUpdate : class
    {
        var entity = await _dbSet.FindAsync([Id], cancellationToken) ?? throw new ObjectNotFoundException();

        entity = mappingHelper.MapTo(update, entity);
        _dbSet.Entry(entity).State = EntityState.Modified;
        _dbSet.Update(entity);

        return entity;
    }

    public async Task DeleteByFilterAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        await _dbSet.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<bool> DoesExistAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(filter, cancellationToken);
    }
}
