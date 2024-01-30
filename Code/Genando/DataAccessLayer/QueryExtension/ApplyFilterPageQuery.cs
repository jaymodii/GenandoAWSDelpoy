using Entities.DTOs.Request;

namespace DataAccessLayer.QueryExtension;
public static class ApplyFilterPageQuery
{
    public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> query, PageRequestDto request)
    {
        int pageSize = request.PageSize;
        int pageNumber = request.PageNumber;

        int skip = (pageNumber - 1) * pageSize;

        return
            query
            .Skip(skip)
            .Take(pageSize);
    }
}
