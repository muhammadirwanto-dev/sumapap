using System.Linq.Expressions;
using Sumapap.Persistence.Abstraction;
using Sumapap.Queries;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Persistence.Specifications
{
    public class PagingSpecification<T> : BaseSpecification<T>
    {
        public PagingSpecification(int page, int pageSize, SortDirection sortDirection = SortDirection.Asc, IList<string>? includes = null)
            : base(includes ?? [])
        {
            var query = new Query(offsetPaging: new OffsetPaginationOptions(page, pageSize));

            if (typeof(IEntity<>).IsAssignableFrom(typeof(T)))
            {
                // sort by Id by default if T implements IEntity<TKey>
                query.Sort.By(nameof(IEntity<>.Id), sortDirection);
            }

            SetQuery(query);
        }

        public PagingSpecification(string cursorField, string? cursor = null, int limit = 20, CursorDirection direction = CursorDirection.Forward,
            SortDirection sortDirection = SortDirection.Asc,
            IList<string>? includes = null)
            : base(includes ?? [])
        {
            var query = new Query(cursorPaging: new CursorPaginationOptions(cursorField, cursor, limit, direction));

            if (typeof(IEntity<>).IsAssignableFrom(typeof(T)))
            {
                // sort by Id by default if T implements IEntity<TKey>
                query.Sort.By(nameof(IEntity<>.Id), sortDirection);
            }

            SetQuery(query);
        }

        public PagingSpecification(Expression<Func<T, bool>> criteria, OffsetPaginationOptions options, SortOptions? sort = null,
            IList<string>? includes = null)
            : base(criteria, includes ?? [])
        {
            SetQuery(new Query(sort ?? SortOptions.Empty, offsetPaging: options));
        }

        public PagingSpecification(Expression<Func<T, bool>> criteria, CursorPaginationOptions options, SortOptions? sort = null,
            IList<string>? includes = null)
            : base(criteria, includes ?? [])
        {
            SetQuery(new Query(sort ?? SortOptions.Empty, cursorPaging: options));
        }
    }
}
