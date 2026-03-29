using System.Linq.Expressions;
using Sumapap.Queries.Execution.Cursor;
using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryableCursorPaging
    {
        public static QueryResult<T> Apply<T>(IQueryable<T> source, Query query)
        {
            var paging = query.CursorPaging!;
            source = ApplyOrdering(source, paging);

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                source = ApplyCursorFilter(source, paging);
            }

            var items = source.Take(paging.Limit + 1).ToList();
            var hasNext = items.Count > paging.Limit;
            var result = items.Take(paging.Limit).ToList();

            var endCursor = result.Count > 0
                ? CursorEncryption.EncodeCursor(
                    ReflectionCache
                        .GetProperty<T>(paging.CursorField)!
                        .GetValue(result.Last())!)
                : null;

            return new QueryResult<T>(
                result,
                -1,
                new PageInfo(
                    hasNextPage: hasNext,
                    hasPreviousPage: paging.Cursor != null,
                    paging.Cursor,
                    endCursor)
            );
        }

        private static IQueryable<T> ApplyOrdering<T>(
            IQueryable<T> source,
            CursorPaginationOptions paging)
        {
            var prop = ReflectionCache.GetProperty<T>(paging.CursorField)!;
            var param = Expression.Parameter(typeof(T), "e");
            var body = Expression.Property(param, prop);
            var lambda = Expression.Lambda(body, param);

            var methodName = paging.Direction == CursorDirection.Forward
                ? nameof(System.Linq.Queryable.OrderBy)
                : nameof(System.Linq.Queryable.OrderByDescending);

            var method = typeof(System.Linq.Queryable)
                .GetMethods()
                .Single(m =>
                    m.Name == methodName &&
                    m.GetParameters().Length == 2);

            return (IQueryable<T>)method
                .MakeGenericMethod(typeof(T), prop.PropertyType)
                .Invoke(null, [source, lambda])!;
        }

        private static IQueryable<T> ApplyCursorFilter<T>(
            IQueryable<T> source,
            CursorPaginationOptions paging)
        {
            var prop = ReflectionCache.GetProperty<T>(paging.CursorField)!;
            var cursorValue = CursorEncryption.DecodeCursor(
                paging.Cursor!,
                prop.PropertyType);

            var param = Expression.Parameter(typeof(T), "e");
            var left = Expression.Property(param, prop);
            var right = Expression.Constant(cursorValue, prop.PropertyType);

            var comparison = paging.Direction == CursorDirection.Forward
                ? Expression.GreaterThan(left, right)
                : Expression.LessThan(left, right);

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, param);
            return source.Where(lambda);
        }
    }
}
