using DataAccessLayer.Helpers;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Abstraction;

public interface IGenericService<T> where T : class
{
    Task AddAsync(T model,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> models,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(T model,
        CancellationToken cancellationToken = default);

    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity);

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, T>>? select = null);

    Task<(long count, IEnumerable<T> data)> GetAllAsync(PageFilterCriteria<T> criteria,
    CancellationToken cancellationToken = default);

    Task<T?> GetAsync(FilterCriteria<T> criteria,
        CancellationToken cancellationToken = default);
}