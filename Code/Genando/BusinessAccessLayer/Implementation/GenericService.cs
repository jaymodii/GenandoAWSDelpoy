using BusinessAccessLayer.Abstraction;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Implementation;

public class GenericService<T> : IGenericService<T> where T : class
{
    #region Properties

    private readonly IGenericRepository<T> _repository;
    private readonly IUnitOfWork _unitOfWork;

    #endregion Properties

    #region Constructor

    public GenericService(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    #endregion Constructor

    #region Methods

    public async virtual Task AddAsync(T model, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(model, cancellationToken);
        await _unitOfWork.SaveAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> models,
        CancellationToken cancellationToken = default)
        => await _repository.AddRangeAsync(models, cancellationToken);

    public async virtual Task RemoveAsync(T model,
        CancellationToken cancellationToken = default)
    {
         await _repository.RemoveAsync(model, cancellationToken);
         await _unitOfWork.SaveAsync();
    }

    public async virtual Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => await _repository.GetFirstOrDefaultAsync(filter, cancellationToken);

    public async virtual Task<bool> AnyAsync(Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => await _repository.AnyAsync(filter, cancellationToken);

    public async Task UpdateRangeAsync(IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        _repository.UpdateRange(entities);
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _repository.Update(entity);
        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        => await _repository.UpdateAsync(entity, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, T>>? select = null)
        => await _repository.GetAllAsync(filter, select);

    public async Task<(long count, IEnumerable<T> data)> GetAllAsync(PageFilterCriteria<T> criteria,
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(criteria, cancellationToken);

    public async Task<T?> GetAsync(FilterCriteria<T> criteria,
        CancellationToken cancellationToken = default)
        => await _repository.GetAsync(criteria, cancellationToken);

    public async Task RemoveRangeAsync(IEnumerable<T> entities,
     CancellationToken cancellationToken = default)
    {
        _repository.RemoveRange(entities);
        await _unitOfWork.SaveAsync();
    }

    #endregion Methods
}