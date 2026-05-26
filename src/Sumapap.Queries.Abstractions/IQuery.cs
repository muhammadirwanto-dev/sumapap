using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;

namespace Sumapap.Queries.Abstractions
{
    /// <summary>
    /// Represents a query with filtering, sorting, and pagination configuration.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Gets the filter configuration for the query. Never null; defaults to <see cref="FilterConfiguration.Empty"/>.
        /// </summary>
        FilterConfiguration Filters { get; }

        /// <summary>
        /// Gets the sort configuration for the query. Never null; defaults to empty configuration.
        /// </summary>
        SortConfiguration Sort { get; }

        /// <summary>
        /// Gets the offset-based pagination configuration, or null if not using offset pagination.
        /// </summary>
        OffsettPaginationConfiguration? OffsetPaging { get; }

        /// <summary>
        /// Gets the cursor-based pagination configuration, or null if not using cursor pagination.
        /// </summary>
        CursorPaginationConfiguration? CursorPaging { get; }

        /// <summary>
        /// Gets a value indicating whether this query uses cursor-based pagination.
        /// </summary>
        bool UsesCursorPaging { get; }

        /// <summary>
        /// Gets a value indicating whether this query uses offset-based pagination.
        /// </summary>
        bool UsesOffsetPaging { get; }
    }
}
