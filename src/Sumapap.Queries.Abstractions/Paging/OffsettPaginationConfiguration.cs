namespace Sumapap.Queries.Abstractions.Paging
{
    /// <summary>
    /// Configures traditional offset-based (page number) pagination.
    /// </summary>
    public sealed class OffsettPaginationConfiguration(
        int page = 1,
        int pageSize = 20)
    {
        /// <summary>
        /// Gets the page number (1-based).
        /// </summary>
        public int Page { get; } = page;

        /// <summary>
        /// Gets the number of items per page.
        /// </summary>
        public int PageSize { get; } = pageSize;

        /// <summary>
        /// Gets the calculated offset (number of items to skip).
        /// </summary>
        public int Offset => (Page - 1) * PageSize;
    }
}
