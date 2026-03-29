using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries
{
    public sealed class Query : IQuery
    {
        public FilterOptions Filters { get; }

        public SortOptions Sort { get; }

        public OffsetPaginationOptions? OffsetPaging { get; }

        public CursorPaginationOptions? CursorPaging { get; }

        public bool UsesCursorPaging => CursorPaging != null;

        public bool UsesOffsetPaging => OffsetPaging != null;

        public Query()
        {
            Filters = FilterOptions.Empty;
            Sort = SortOptions.Empty;
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

        public Query(
            FilterOptions filters,
            SortOptions sort,
            OffsetPaginationOptions? offsetPaging = null,
            CursorPaginationOptions? cursorPaging = null)
        {
            Filters = filters;
            Sort = sort;
            OffsetPaging = offsetPaging;
            CursorPaging = cursorPaging;
        }
    }
}
