using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Abstractions
{
    public interface IQuery
    {
        FilterConfiguration Filters { get; }

        SortConfiguration Sort { get; }

        OffsettPaginationConfiguration? OffsetPaging { get; }

        CursorPaginationConfiguration? CursorPaging { get; }

        bool UsesCursorPaging { get; }

        bool UsesOffsetPaging { get; }
    }
}
