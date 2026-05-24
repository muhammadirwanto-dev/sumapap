using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;

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
