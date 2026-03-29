using System.Linq.Expressions;

namespace LearnWellUniversity_CMS.Application.Abstractions;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<T> GetAll();
    IQueryable<T> GetByFilters(Expression<Func<T, bool>>? filter = null, int? page = 0, int? pageSize = null);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}