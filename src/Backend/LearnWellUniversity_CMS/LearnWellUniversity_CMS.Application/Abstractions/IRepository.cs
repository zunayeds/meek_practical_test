using System.Linq.Expressions;

namespace LearnWellUniversity_CMS.Application.Abstractions;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
    IQueryable<T> GetAllAsQueryable();
    IQueryable<T> GetByFiltersAsQueryable(Expression<Func<T, bool>>? filter = null, int? page = 0, int? pageSize = null);
    Task<List<TResponseType>> GetByFiltersAsync<TResponseType>(Expression<Func<T, bool>>? filter = null, int? page = 0, int? pageSize = null, CancellationToken cancellationToken = default) where TResponseType : class;
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> Update<TUpdate>(Guid Id, TUpdate update, CancellationToken cancellationToken = default) where TUpdate : class;
    Task DeleteByFilterAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
    Task<bool> DoesExistAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
}