using DataAccessLayer.Helpers;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using System.Linq.Expressions;

namespace DataAccessLayer.Abstraction;
public interface IGenericRepository<T> where T : class
{
    Task AddAsync(T model,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(T model,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> models,
        CancellationToken cancellationToken = default);

    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(T model,
        CancellationToken cancellationToken = default);

    void UpdateRange(IEnumerable<T> entities);

    void Update(T entity);

    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate);

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, T>>? select = null);

    Task<(long count, IEnumerable<T> data)> GetAllAsync(PageFilterCriteria<T> criteria,
        CancellationToken cancellationToken = default);

    Task<PageListResponseDTO<T>> GetAllAsync(PageListRequestEntity<T> pageListRequest, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<List<T>> GetAllDataAsync(Expression<Func<T, bool>> filter = null,
       Expression<Func<T, object>>[]? includes = null, string[]? thenIncludes = null, Expression<Func<T, T>>? select = null);

    Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> IsEntityExist(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<T?> GetAsync(Expression<Func<T, bool>> expression,
        Expression<Func<T, object>>[]? includes = null, string[]? thenIncludes = null,
        Expression<Func<T, T>>? selects = null,
        CancellationToken cancellationToken = default);

    Task<T?> GetAsync(FilterCriteria<T> criteria,
        CancellationToken cancellationToken = default);

    void RemoveRange(IEnumerable<T> entities);
}