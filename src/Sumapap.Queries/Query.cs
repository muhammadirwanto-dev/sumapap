using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries
{
    public sealed class Query(
        FilterOptions filters,
        SortOptions sort,
        OffsetPaginationOptions? offsetPaging = null,
        CursorPaginationOptions? cursorPaging = null) : IQuery
    {
        public FilterOptions Filters { get; } = filters;

        public SortOptions Sort { get; } = sort;

        public OffsetPaginationOptions? OffsetPaging { get; } = offsetPaging;

        public CursorPaginationOptions? CursorPaging { get; } = cursorPaging;

        public bool UsesCursorPaging => CursorPaging != null;

        public bool UsesOffsetPaging => OffsetPaging != null;

        public Query()
            : this(FilterOptions.Empty, SortOptions.Empty, null, null)
        {
        }

        public Query(
            OffsetPaginationOptions offsetPaging)
            : this(FilterOptions.Empty, SortOptions.Empty, offsetPaging, null)
        {
        }

        public Query(
            CursorPaginationOptions cursorPaging)
            : this(FilterOptions.Empty, SortOptions.Empty, null, cursorPaging)
        {
        }

        public Query(
            SortOptions sort,
            OffsetPaginationOptions? offsetPaging = null,
            CursorPaginationOptions? cursorPaging = null)
            : this(FilterOptions.Empty, sort, offsetPaging, cursorPaging)
        {
        }

        public Query(
            FilterOptions filters,
            OffsetPaginationOptions? offsetPaging = null,
            CursorPaginationOptions? cursorPaging = null)
            : this(filters, SortOptions.Empty, offsetPaging, cursorPaging)
        {
        }
    }
}
