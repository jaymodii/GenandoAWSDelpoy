using DataAccessLayer.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.QueryExtension;

public static class ExpressionEvaluator
{
    public static async Task<(long count, IEnumerable<T> data)> EvaluatePageQuery<T>(this IQueryable<T> query,
        PageFilterCriteria<T> criteria,
        CancellationToken cancellationToken = default) where T : class
    {
        query = query.IncludeExpressions(criteria.IncludeExpressions);
        query = query.FilterQuery(criteria);

        long count = await query.CountAsync(cancellationToken);

        query = ApplyOrdering(query, criteria);

        if (criteria.PageRequest is not null)
            query = query.ApplyQuery(criteria.PageRequest);

        return (count, query.ApplySelect(criteria.Select));
    }

    private static IQueryable<T> ApplyOrdering<T>(IQueryable<T> query,
        PageFilterCriteria<T> criteria)
        where T : class
    {
        if (criteria.OrderBy is not null)
            query = query.OrderBy(criteria.OrderBy);

        if (criteria.OrderByDescending is not null)
            query = query.OrderByDescending(criteria.OrderByDescending);

        return query;
    }

    public static IEnumerable<T> ApplySelect<T>(this IQueryable<T> query,
        Expression<Func<T, T>>? select = null) where T : class
        => select is null ? query.AsEnumerable() : query.Select(select).AsEnumerable();

    public static IQueryable<T> FilterQuery<T>(this IQueryable<T> query,
        PageFilterCriteria<T> criteria) where T : class
    {
        if (criteria.StatusFilter is not null) query = query.Where(criteria.StatusFilter);

        if (criteria.Filter is not null) query = query.Where(criteria.Filter);
        return query;
    }

    public static IQueryable<T> FilterQuery<T>(this IQueryable<T> query,
        Expression<Func<T, bool>>? filter = null) where T : class
    {
        if (filter is not null) query = query.Where(filter);
        return query;
    }

    public static IQueryable<T> IncludeExpressions<T>(this IQueryable<T> query,
        List<Expression<Func<T, object>>>? includeExpressions = null) where T : class
    {
        if (includeExpressions is null) return query;

        query = includeExpressions.Aggregate(query, (current, include) =>
        {
            return current.Include(include);
        });

        return query;
    }

    public static IQueryable<T> EvaluateQuery<T>(this IQueryable<T> query,
        FilterCriteria<T> criteria) where T : class
    {
        query = query.IncludeExpressions(criteria.IncludeExpressions);
        query = query.FilterQuery(criteria.Filter);
        query = query.FilterQuery(criteria.StatusFilter);

        return criteria.Select is not null ? query.Select(criteria.Select) : query;
    }
}