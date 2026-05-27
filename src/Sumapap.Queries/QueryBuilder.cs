using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;

namespace Sumapap.Queries
{
    /// <summary>
    /// Provides a fluent API for building <see cref="IQuery"/> instances.
    /// </summary>
    public class QueryBuilder
    {
        private FilterConfiguration _filters = new();
        private SortConfiguration _sort = new();
        private OffsettPaginationConfiguration? _offset;
        private CursorPaginationConfiguration? _cursor;

        /// <summary>
        /// Sets the filter configuration for the query.
        /// </summary>
        /// <param name="filters">The filter configuration.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder WithFilters(FilterConfiguration filters) { _filters = filters; return this; }

        /// <summary>
        /// Sets the sort configuration for the query.
        /// </summary>
        /// <param name="sort">The sort configuration.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder WithSort(SortConfiguration sort) { _sort = sort; return this; }

        /// <summary>
        /// Enables offset-based pagination with the specified configuration.
        /// </summary>
        /// <param name="paging">The offset pagination configuration.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder UseOffsetPaging(OffsettPaginationConfiguration paging)
        {
            _cursor = null;
            _offset = paging;

            return this;
        }

        /// <summary>
        /// Enables offset-based pagination with the specified limit and offset.
        /// </summary>
        /// <param name="limit">The maximum number of items per page.</param>
        /// <param name="offset">The number of items to skip.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder UseOffsetPaging(int limit, int offset)
            => UseOffsetPaging(new OffsettPaginationConfiguration(limit, offset));

        /// <summary>
        /// Enables cursor-based pagination with the specified configuration.
        /// </summary>
        /// <param name="paging">The cursor pagination configuration.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder UseCursorPaging(CursorPaginationConfiguration paging)
        {
            _offset = null;
            _cursor = paging;

            return this;
        }

        /// <summary>
        /// Enables cursor-based pagination with the specified parameters.
        /// </summary>
        /// <param name="cursorField">The field to use for cursor comparison.</param>
        /// <param name="cursor">The cursor value, or null for the first page.</param>
        /// <param name="limit">The maximum number of items per page.</param>
        /// <param name="direction">The pagination direction.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder UseCursorPaging(string cursorField, string? cursor = null, int limit = 20, CursorDirection direction = CursorDirection.Forward)
            => UseCursorPaging(new CursorPaginationConfiguration(cursorField, cursor, limit, direction));

        /// <summary>
        /// Sets the filter configuration if not null.
        /// </summary>
        /// <param name="filters">The filter configuration, or null to skip.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder WithOptionalFilter(FilterConfiguration? filters)
            => filters == null ? this : WithFilters(filters);

        /// <summary>
        /// Sets the sort configuration if not null.
        /// </summary>
        /// <param name="sort">The sort configuration, or null to skip.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public QueryBuilder WithOptionalSort(SortConfiguration? sort)
            => sort == null ? this : WithSort(sort);

        /// <summary>
        /// Builds the final <see cref="IQuery"/> instance.
        /// </summary>
        /// <returns>A new query with the configured parameters.</returns>
        public IQuery Build() => new Query(_filters, _sort, _offset, _cursor);
    }
}
