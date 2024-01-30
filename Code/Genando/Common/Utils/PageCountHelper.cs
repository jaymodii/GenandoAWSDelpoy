namespace Common.Utils;
public static class PageCountHelper
{
    public static int GetTotalPage(long count,
        int pageSize)
        => (int)Math.Ceiling((double)count / pageSize);
}
