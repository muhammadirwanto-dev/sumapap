using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Paging;

namespace Sumapap.Queries
{
    /// <summary>
    /// Default implementation of <see cref="IQueryResult{T}"/> containing query results and metadata.
    /// </summary>
    /// <typeparam name="T">The type of items in the result set.</typeparam>
    public sealed class QueryResult<T>(
        IEnumerable<T> items,
        int totalDataCount = 0,
        PageInfo? pageInfo = null) : IQueryResult<T>
    {
        /// <summary>
        /// Gets the items in the current page of results.
        /// </summary>
        public IEnumerable<T> Items { get; } = items;

        /// <summary>
        /// Gets the total count of items matching the query (before pagination).
        /// </summary>
        public int TotalDataCount { get; } = totalDataCount;

        /// <summary>
        /// Gets the pagination metadata, or null for non-paginated results.
        /// </summary>
        public PageInfo? PageInfo { get; } = pageInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class with empty results.
        /// </summary>
        public QueryResult()
            : this([], 0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class with a specified total count.
        /// </summary>
        /// <param name="totalDataCount">The total count of items.</param>
        public QueryResult(int totalDataCount)
            : this([], totalDataCount, null)
        {
        }
    }
}
