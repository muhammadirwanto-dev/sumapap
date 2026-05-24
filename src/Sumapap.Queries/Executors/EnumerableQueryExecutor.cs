using System.Collections;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;
using Sumapap.Queries.Internal;
using Sumapap.Queries.Utils;

namespace Sumapap.Queries.Executors
{
    public sealed class EnumerableQueryExecutor<T>
        : QueryExecutorBase<IEnumerable<T>, T>
    {
        protected override IEnumerable<T> ApplyFiltering(IEnumerable<T> source, IQuery query)
        {
            var expr = _expressionBuilder.BuildFilterExpression(query.Filters);

            return source.AsEnumerable().Where(expr.Compile()).AsEnumerable();
        }

        protected override IEnumerable<T> ApplySorting(IEnumerable<T> source, IQuery query)
        {
            var sorts = query.Sort?.Sorts;
            if (sorts == null || !sorts.Any())
            {
                return source;
            }

            IOrderedEnumerable<T>? ordered = null;

            foreach (var descriptor in sorts)
            {
                var expression = _expressionBuilder.BuildSortExpression(descriptor.Field);

                ordered = ordered == null
                    ? descriptor.Direction == SortDirection.Asc
                        ? source.OrderBy(expression.Compile())
                        : source.OrderByDescending(expression.Compile())
                    : descriptor.Direction == SortDirection.Asc
                        ? ordered.ThenBy(expression.Compile())
                        : ordered.ThenByDescending(expression.Compile());
            }

            return ordered ?? source;
        }

        protected override IQueryResult<T> ApplyCursorPaging(IEnumerable<T> source, IQuery query)
        {
            var paging = query.CursorPaging!;
            var prop = typeof(T).GetProperty(paging.CursorField)!;
            var lambda = _expressionBuilder.BuildSortExpression(paging.CursorField);

            // Re-use sorting logic to ensure cursor stability
            source = paging.Direction == CursorDirection.Forward
                ? source.OrderBy(lambda.Compile())
                : source.OrderByDescending(lambda.Compile());

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                var cursorValue = CursorEncryption.DecodeCursor(paging.Cursor, prop.PropertyType);

                source = paging.Direction == CursorDirection.Forward
                    ? source.Where(x => Comparer.Default.Compare(prop.GetValue(x), cursorValue) > 0)
                    : source.Where(x => Comparer.Default.Compare(prop.GetValue(x), cursorValue) < 0);
            }

            var items = source.Take(paging.Limit + 1);
            var hasNext = items.Count() > paging.Limit;
            var result = items.Take(paging.Limit);

            var endCursor = result.Any()
                ? CursorEncryption.EncodeCursor(prop.GetValue(result.Last())!)
                : null;

            return new QueryResult<T>(
                result,
                totalDataCount: -1,
                pageInfo: new PageInfo(
                    hasNextPage: hasNext,
                    hasPreviousPage: paging.Cursor != null,
                    startCursor: paging.Cursor,
                    endCursor
                )
            );
        }
    }
}
