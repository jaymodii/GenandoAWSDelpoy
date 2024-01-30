using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Helpers;
using DataAccessLayer.QueryExtension;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Implementation;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    #region Properties

    protected readonly AppDbContext _dbContext;

    private readonly DbSet<T> _dbSet;

    #endregion Properties

    #region Constructor

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    #endregion Constructor

    #region Interface methods

    public virtual async Task AddAsync(T model,
        CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(model, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<T> models,
        CancellationToken cancellationToken = default)
        => await _dbSet.AddRangeAsync(models, cancellationToken);

    public async Task UpdateAsync(T model,
        CancellationToken cancellationToken = default)
        => await Task.Run(() => _dbSet.Update(model), cancellationToken);

    public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(filter, cancellationToken);

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(filter, cancellationToken);

    public virtual async Task RemoveAsync(T model,
        CancellationToken cancellationToken = default)
        => await Task.Run(() => _dbSet.Remove(model), cancellationToken);

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate)
    {
        if (predicate == null)
        {
            return await _dbSet.ToListAsync();
        }

        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, T>>? select = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking().AsQueryable();

        if (filter is not null)
            query = query.FilterQuery(filter);

        return await Task.Run(() => select is null ? query.AsEnumerable()
            : query.ApplySelect(select).AsEnumerable());
    }

    public async Task<(long count, IEnumerable<T> data)> GetAllAsync(PageFilterCriteria<T> criteria,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(async () =>
        {
            IQueryable<T> query = GetAll();

            (long count, IEnumerable<T> data) result = await query.EvaluatePageQuery(criteria, cancellationToken);

            return result;
        }, cancellationToken);
    }

    public async Task<PageListResponseDTO<T>> GetAllAsync(PageListRequestEntity<T> pageListRequest, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsQueryable();

        if (pageListRequest.IncludeExpressions != null)
        {
            query = pageListRequest.IncludeExpressions.Aggregate(query, (current, include) =>
            {
                return current.Include(include);
            });
        }

        if (pageListRequest.ThenIncludeExpressions != null)
        {
            query = pageListRequest.ThenIncludeExpressions.Aggregate(query, (current, thenInclude) =>
            {
                return current.Include(thenInclude);
            });
        }

        if (pageListRequest.Selects != null)
            query = query.Select(pageListRequest.Selects);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if(pageListRequest.OrderByDescending != null)
        {
            query = query.OrderByDescending(pageListRequest.OrderByDescending);
        }

        int totalRecords = await query.CountAsync(cancellationToken);

        var records = await query
        .Skip((pageListRequest.PageIndex - 1) * pageListRequest.PageSize)
        .Take(pageListRequest.PageSize)
        .ToListAsync(cancellationToken);

        return new PageListResponseDTO<T>(pageListRequest.PageIndex, pageListRequest.PageSize, totalRecords, records);
    }

    public async Task<List<T>> GetAllDataAsync(Expression<Func<T, bool>>? filter = null,
       Expression<Func<T, object>>[]? includes = null, string[]? thenIncludes = null, Expression<Func<T, T>>? select = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking().AsQueryable();

        if (filter is not null)
            query = query.FilterQuery(filter);

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) =>
            {
                return current.Include(include);
            });
        }

        if (thenIncludes != null)
        {
            query = thenIncludes.Aggregate(query, (current, thenInclude) =>
            {
                return current.Include(thenInclude);
            });
        }

        return await Task.Run(() => select is null ? query.ToList()
            : query.ApplySelect(select).ToList());
    }


    public async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<bool> IsEntityExist(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object>>[]? includes = null, string[]? thenIncludes = null, Expression<Func<T, T>>? selects = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsQueryable();

        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) =>
            {
                return current.Include(include);
            });
        }

        if (thenIncludes != null)
        {
            query = thenIncludes.Aggregate(query, (current, thenInclude) =>
            {
                return current.Include(thenInclude);
            });
        }

        query = query.Where(expression);

        if (selects != null)
            query = query.Select(selects);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetAsync(FilterCriteria<T> criteria,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsQueryable().EvaluateQuery(criteria);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }


    #endregion Interface methods

    #region Helper Methods

    private IQueryable<T> GetAll()
        => _dbSet.AsNoTracking().AsQueryable();

    #endregion Helper Methods
}