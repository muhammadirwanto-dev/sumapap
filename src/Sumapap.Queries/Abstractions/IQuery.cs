using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Abstractions
{
    public interface IQuery
    {
        FilterOptions Filters { get; }

        SortOptions Sort { get; }

        OffsetPaginationOptions? OffsetPaging { get; }

        CursorPaginationOptions? CursorPaging { get; }

        bool UsesCursorPaging { get; }

        bool UsesOffsetPaging { get; }
    }
}
