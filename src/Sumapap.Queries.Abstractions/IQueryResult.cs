using Sumapap.Queries.Abstractions.Paging;

namespace Sumapap.Queries.Abstractions
{
    /// <summary>
    /// Represents the result of a query execution, containing data and pagination metadata.
    /// </summary>
    /// <typeparam name="T">The type of items in the result set.</typeparam>
    public interface IQueryResult<out T>
    {
        /// <summary>
        /// Gets the items in the current page of results.
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Gets the total count of items matching the query (before pagination).
        /// </summary>
        int TotalDataCount { get; }

        /// <summary>
        /// Gets the pagination metadata, or null if pagination was not applied.
        /// </summary>
        PageInfo? PageInfo { get; }
    }
}
