namespace Entities.DTOs.Request
{
    public class BaseListRequestDTO
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public BaseListRequestDTO()
        {
            PageIndex = PageIndex < 1 ? 1 : PageIndex;
            PageSize = PageSize < 1 ? 10 : PageSize;
        }
    }
}
