namespace Sumapap.Queries.Abstractions.Paging
{
    public sealed class PageInfo(
        bool hasNextPage,
        bool hasPreviousPage,
        string? startCursor = null,
        string? endCursor = null)
    {
        public bool HasNextPage { get; } = hasNextPage;

        public bool HasPreviousPage { get; } = hasPreviousPage;

        public string? StartCursor { get; } = startCursor;

        public string? EndCursor { get; } = endCursor;
    }
}
