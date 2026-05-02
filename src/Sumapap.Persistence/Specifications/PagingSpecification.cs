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
            var query = new QueryBuilder()
                .UseOffsetPaging(page, pageSize)
                .Build();

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
            var query = new QueryBuilder()
                .UseCursorPaging(cursorField, cursor, limit, direction)
                .Build();

            if (typeof(IEntity<>).IsAssignableFrom(typeof(T)))
            {
                // sort by Id by default if T implements IEntity<TKey>
                query.Sort.By(nameof(IEntity<>.Id), sortDirection);
            }

            SetQuery(query);
        }

        public PagingSpecification(Expression<Func<T, bool>> criteria, OffsettPaginationConfiguration options, SortConfiguration? sort = null,
            IList<string>? includes = null)
            : base(criteria, includes ?? [])
        {
            SetQuery(new QueryBuilder()
                .UseOffsetPaging(options)
                .WithOptionalSort(sort)
                .Build());
        }

        public PagingSpecification(Expression<Func<T, bool>> criteria, CursorPaginationConfiguration options, SortConfiguration? sort = null,
            IList<string>? includes = null)
            : base(criteria, includes ?? [])
        {
            SetQuery(new QueryBuilder()
                .UseCursorPaging(options)
                .WithOptionalSort(sort)
                .Build());
        }
    }
}
