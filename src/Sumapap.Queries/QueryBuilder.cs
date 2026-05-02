using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries
{
    public class QueryBuilder
    {
        private FilterConfiguration _filters = new();
        private SortConfiguration _sort = new();
        private OffsettPaginationConfiguration? _offset;
        private CursorPaginationConfiguration? _cursor;

        public QueryBuilder WithFilters(FilterConfiguration filters) { _filters = filters; return this; }

        public QueryBuilder WithSort(SortConfiguration sort) { _sort = sort; return this; }

        public QueryBuilder UseOffsetPaging(OffsettPaginationConfiguration paging)
        {
            _cursor = null;
            _offset = paging;

            return this;
        }

        public QueryBuilder UseCursorPaging(CursorPaginationConfiguration paging)
        {
            _offset = null;
            _cursor = paging;

            return this;
        }

        public QueryBuilder WithOptionalFilter(FilterConfiguration? filters)
            => filters == null ? this : WithFilters(filters);

        public QueryBuilder WithOptionalSort(SortConfiguration? sort)
            => sort == null ? this : WithSort(sort);

        public QueryBuilder UseOffsetPaging(int limit, int offset)
            => UseOffsetPaging(new OffsettPaginationConfiguration(limit, offset));

        public QueryBuilder UseCursorPaging(string cursorField, string? cursor = null, int limit = 20, CursorDirection direction = CursorDirection.Forward)
            => UseCursorPaging(new CursorPaginationConfiguration(cursorField, cursor, limit, direction));

        public Query Build() => new(_filters, _sort, _offset, _cursor);
    }
}
