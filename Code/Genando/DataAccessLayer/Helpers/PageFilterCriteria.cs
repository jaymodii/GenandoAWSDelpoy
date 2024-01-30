using Entities.DTOs.Request;
using System.Linq.Expressions;

namespace DataAccessLayer.Helpers;

public class PageFilterCriteria<T>
{
    public PageRequestDto PageRequest { get; set; } = null!;

    public Expression<Func<T, bool>>? Filter { get; set; } = null;

    public List<Expression<Func<T, object>>>? IncludeExpressions { get; set; } = null;

    public Expression<Func<T, bool>>? StatusFilter { get; set; } = null;

    public Expression<Func<T, T>>? Select { get; set; } = null;

    public Expression<Func<T, object>>? OrderBy { get; set; } = null;

    public Expression<Func<T, object>>? OrderByDescending { get; set; } = null;
}