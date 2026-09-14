using Sumapap.Persistence.Abstractions.Entities;
using Sumapap.Queries;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;
using System.Linq.Expressions;

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
#if NET10_0_OR_GREATER
                query.Sort.By(nameof(IEntity<>.Id), sortDirection);
#else
                query.Sort.By(nameof(IEntity<Guid>.Id), sortDirection);
#endif
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
#if NET10_0_OR_GREATER
                query.Sort.By(nameof(IEntity<>.Id), sortDirection);
#else
                query.Sort.By(nameof(IEntity<Guid>.Id), sortDirection);
#endif
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
