using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;

namespace Sumapap.Queries.Internal
{
    public sealed class Query(
        FilterConfiguration filters,
        SortConfiguration sort,
        OffsettPaginationConfiguration? offsetPaging = null,
        CursorPaginationConfiguration? cursorPaging = null) : IQuery
    {
        public FilterConfiguration Filters { get; } = filters;

        public SortConfiguration Sort { get; } = sort;

        public OffsettPaginationConfiguration? OffsetPaging { get; } = offsetPaging;

        public CursorPaginationConfiguration? CursorPaging { get; } = cursorPaging;

        public bool UsesCursorPaging => CursorPaging != null;

        public bool UsesOffsetPaging => OffsetPaging != null;

        public Query()
            : this(FilterConfiguration.Empty, SortConfiguration.Empty, null, null)
        {
        }
    }
}
