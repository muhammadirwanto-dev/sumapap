namespace Sumapap.Queries.Abstractions.Paging
{
    /// <summary>
    /// Contains pagination metadata for query results.
    /// </summary>
    public sealed class PageInfo(
        bool hasNextPage,
        bool hasPreviousPage,
        string? startCursor = null,
        string? endCursor = null)
    {
        /// <summary>
        /// Gets a value indicating whether there are more items available after the current page.
        /// </summary>
        public bool HasNextPage { get; } = hasNextPage;

        /// <summary>
        /// Gets a value indicating whether there are items available before the current page.
        /// </summary>
        public bool HasPreviousPage { get; } = hasPreviousPage;

        /// <summary>
        /// Gets the cursor pointing to the start of the current page, or null if not using cursor pagination.
        /// </summary>
        public string? StartCursor { get; } = startCursor;

        /// <summary>
        /// Gets the cursor pointing to the end of the current page, or null if not using cursor pagination.
        /// </summary>
        public string? EndCursor { get; } = endCursor;
    }
}
