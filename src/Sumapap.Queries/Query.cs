using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;

namespace Sumapap.Queries
{
    /// <summary>
    /// Default implementation of <see cref="IQuery"/> that encapsulates query parameters.
    /// </summary>
    public sealed class Query(
        FilterConfiguration filters,
        SortConfiguration sort,
        OffsettPaginationConfiguration? offsetPaging = null,
        CursorPaginationConfiguration? cursorPaging = null) : IQuery
    {
        /// <summary>
        /// Gets the filter configuration for the query.
        /// </summary>
        public FilterConfiguration Filters { get; } = filters;

        /// <summary>
        /// Gets the sort configuration for the query.
        /// </summary>
        public SortConfiguration Sort { get; } = sort;

        /// <summary>
        /// Gets the offset-based pagination configuration, or null if not using offset pagination.
        /// </summary>
        public OffsettPaginationConfiguration? OffsetPaging { get; } = offsetPaging;

        /// <summary>
        /// Gets the cursor-based pagination configuration, or null if not using cursor pagination.
        /// </summary>
        public CursorPaginationConfiguration? CursorPaging { get; } = cursorPaging;

        /// <summary>
        /// Gets a value indicating whether this query uses cursor-based pagination.
        /// </summary>
        public bool UsesCursorPaging => CursorPaging != null;

        /// <summary>
        /// Gets a value indicating whether this query uses offset-based pagination.
        /// </summary>
        public bool UsesOffsetPaging => OffsetPaging != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class with empty filters and sort.
        /// </summary>
        public Query()
            : this(FilterConfiguration.Empty, SortConfiguration.Empty, null, null)
        {
        }
    }
}
