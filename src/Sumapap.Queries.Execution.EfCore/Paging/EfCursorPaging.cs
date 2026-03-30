using System.Linq.Expressions;
using Sumapap.Queries.Execution.Cursor;
using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.EfCore.Paging
{
    internal static class EfCursorPaging
    {
        public static IQueryable<T> ApplyCursorFilter<T>(
            IQueryable<T> source,
            CursorPaginationOptions paging)
            where T : class
        {
            if (string.IsNullOrEmpty(paging.Cursor))
                return source;

            var prop = ReflectionCache.GetProperty<T>(paging.CursorField)
                ?? throw new InvalidOperationException(
                    $"Cursor field '{paging.CursorField}' not found on '{typeof(T).Name}'.");

            var cursorValue = CursorEncryption.DecodeCursor(
                paging.Cursor,
                prop.PropertyType);

            var parameter = Expression.Parameter(typeof(T), "e");
            var left = Expression.Property(parameter, prop);
            var right = Expression.Constant(cursorValue, prop.PropertyType);

            var comparison = paging.Direction == CursorDirection.Forward
                ? Expression.GreaterThan(left, right)
                : Expression.LessThan(left, right);

            var predicate = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            return source.Where(predicate);
        }

        public static IQueryable<T> ApplyCursorOrdering<T>(
            IQueryable<T> source,
            CursorPaginationOptions paging)
            where T : class
        {
            var prop = ReflectionCache.GetProperty<T>(paging.CursorField)
                ?? throw new InvalidOperationException(
                    $"Cursor field '{paging.CursorField}' not found.");

            var parameter = Expression.Parameter(typeof(T), "e");
            var body = Expression.Property(parameter, prop);
            var lambda = Expression.Lambda(body, parameter);

            var methodName = paging.Direction == CursorDirection.Forward
                ? "OrderBy"
                : "OrderByDescending";

            var method = typeof(System.Linq.Queryable)
                .GetMethods()
                .Single(m =>
                    m.Name == methodName &&
                    m.GetParameters().Length == 2);

            var generic = method.MakeGenericMethod(
                typeof(T),
                prop.PropertyType);

            return (IQueryable<T>)generic.Invoke(
                null,
                [source, lambda])!;
        }
    }
}
