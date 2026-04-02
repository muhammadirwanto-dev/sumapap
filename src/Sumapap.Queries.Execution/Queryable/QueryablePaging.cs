using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryablePaging
    {
        public static IQueryResult<T> Apply<T>(
            IQueryable<T> source,
            IQuery query)
        {
            if (query.UsesCursorPaging)
            {
                return QueryableCursorPaging.Apply(source, query);
            }

            var total = source.Count();

            if (query.UsesOffsetPaging)
            {
                var page = query.OffsetPaging!;
                var items = source
                    .Skip(page.Offset)
                    .Take(page.PageSize)
                    .ToList();

                return new QueryResult<T>(items, total, new PageInfo(
                    hasNextPage: page.Offset + page.PageSize < total,
                    hasPreviousPage: page.Offset > 0)
                    );
            }

            return new QueryResult<T>([.. source], total);
        }
    }
}
