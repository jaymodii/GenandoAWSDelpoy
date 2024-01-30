namespace Entities.DTOs.Response;
public record PageInformationDto<T>(long TotalRecords,
    IEnumerable<T> Content,
    int TotalPage,
    int PageNumber,
    int PageSize);
