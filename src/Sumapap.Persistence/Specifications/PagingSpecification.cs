using System.Linq.Expressions;
using Sumapap.Persistence.Specifications;
using Sumapap.Queries;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Persistence.Specification
{
    public class PagingSpecification<T> : BaseSpecification<T>
    {
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
