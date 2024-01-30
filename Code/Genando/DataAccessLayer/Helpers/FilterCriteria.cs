using System.Linq.Expressions;

namespace DataAccessLayer.Helpers;

/// <summary>
/// Represents criteria for filtering, including filter expressions, included navigation properties,
/// status filter, and selection projection.
/// </summary>
/// <typeparam name="T">The entity type for which filtering criteria are defined.</typeparam>
public class FilterCriteria<T> where T : class
{
    /// <summary>
    /// Gets or sets the filter expression used to filter entities.
    /// </summary>
    public Expression<Func<T, bool>>? Filter { get; set; } = null;

    /// <summary>
    /// Gets or sets a list of navigation properties to be included in the query results.
    /// </summary>
    public List<Expression<Func<T, object>>>? IncludeExpressions { get; set; } = null;

    /// <summary>
    /// Gets or sets the status filter expression used to filter entities based on their status.
    /// </summary>
    public Expression<Func<T, bool>>? StatusFilter { get; set; } = null;

    /// <summary>
    /// Gets or sets the selection projection expression used to project selected properties of entities.
    /// </summary>
    public Expression<Func<T, T>>? Select { get; set; } = null;

    public string[]? ThenIncludeExpressions { get; set; }
}