using System.Linq.Expressions;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Abstractions.Sorting;
using Sumapap.Queries.Internal;
using Sumapap.Queries.Utils;

namespace Sumapap.Queries.Executors
{
    public sealed class QueryableQueryExecutor<T>
        : QueryExecutorBase<IQueryable<T>, T>
    {
        protected override IQueryable<T> ApplyFiltering(IQueryable<T> source, IQuery query)
        {
            var expr = _expressionBuilder.BuildFilterExpression(query.Filters);

            return source.Where(expr);
        }

        protected override IQueryable<T> ApplySorting(IQueryable<T> source, IQuery query)
        {
            var sorts = query.Sort?.Sorts;
            if (sorts == null || !sorts.Any())
            {
                return source;
            }

            IOrderedQueryable<T>? ordered = null;

            foreach (var descriptor in sorts)
            {
                var expression = _expressionBuilder.BuildSortExpression(descriptor.Field);

                ordered = ordered == null
                    ? descriptor.Direction == SortDirection.Asc
                        ? source.OrderBy(expression)
                        : source.OrderByDescending(expression)
                    : descriptor.Direction == SortDirection.Asc
                        ? ordered.ThenBy(expression)
                        : ordered.ThenByDescending(expression);
            }

            return ordered ?? source;
        }

        protected override IQueryResult<T> ApplyCursorPaging(IQueryable<T> source, IQuery query)
        {
            var paging = query.CursorPaging!;
            var prop = typeof(T).GetProperty(paging.CursorField)!;
            var lambda = _expressionBuilder.BuildSortExpression(paging.CursorField);

            // Re-use sorting logic to ensure cursor stability
            source = paging.Direction == CursorDirection.Forward
                ? source.OrderBy(lambda)
                : source.OrderByDescending(lambda);

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                var cursorValue = CursorEncryption.DecodeCursor(paging.Cursor, prop.PropertyType);

                var param = ExpressionCache<T>.Param;
                var filter = paging.Direction == CursorDirection.Forward
                    ? Expression.GreaterThan(Expression.Property(param, prop), Expression.Constant(cursorValue))
                    : Expression.LessThan(Expression.Property(param, prop), Expression.Constant(cursorValue));

                source = source.Where(Expression.Lambda<Func<T, bool>>(filter, param));
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
                new PageInfo(
                    hasNextPage: hasNext,
                    hasPreviousPage: paging.Cursor != null,
                    startCursor: paging.Cursor,
                    endCursor)
            );
        }
    }
}
