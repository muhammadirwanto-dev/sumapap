namespace Sumapap.Queries.Paging
{
    public sealed class OffsetPaginationOptions(
        int page = 1,
        int pageSize = 20)
    {
        public int Page { get; } = page;

        public int PageSize { get; } = pageSize;

        public int Offset => (Page - 1) * PageSize;
    }
}
