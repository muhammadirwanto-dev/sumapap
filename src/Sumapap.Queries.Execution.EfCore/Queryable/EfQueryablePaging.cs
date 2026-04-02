using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.EfCore.Queryable
{
    internal static class EfQueryablePaging
    {
        public static IQueryResult<T> Apply<T>(
            IQueryable<T> source,
            IQuery query)
        {
            if (query.UsesCursorPaging)
            {
                return EfQueryableCursorPaging.Apply(source, query);
            }

            var total = source.Count();

            if (query.UsesOffsetPaging)
            {
                var paged = source
                    .Skip(query.OffsetPaging!.Offset)
                    .Take(query.OffsetPaging!.PageSize)
                    .ToList();

                return new QueryResult<T>(paged, total, new PageInfo(
                    hasNextPage: query.OffsetPaging.Offset + query.OffsetPaging.PageSize < total,
                    hasPreviousPage: query.OffsetPaging.Offset > 0)
                    );
            }

            return new QueryResult<T>([.. source], total);
        }
    }
}
