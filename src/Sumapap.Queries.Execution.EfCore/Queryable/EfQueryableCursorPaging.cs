using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Common;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.EfCore.Queryable
{
    internal static class EfQueryableCursorPaging
    {
        public static IQueryResult<T> Apply<T>(IQueryable<T> source, IQuery query)
        {
            var paging = query.CursorPaging!;

            source = ApplyCursorOrdering(source, paging);

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                source = ApplyCursorFiltering(source, paging);
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

        private static IQueryable<T> ApplyCursorOrdering<T>(
            IQueryable<T> source,
            CursorPaginationOptions paging)
        {
            return paging.Direction == CursorDirection.Forward
                ? source.OrderBy(e => EF.Property<object>(e!, paging.CursorField))
                : source.OrderByDescending(e => EF.Property<object>(e!, paging.CursorField));
        }

        private static IQueryable<T> ApplyCursorFiltering<T>(
            IQueryable<T> source,
            CursorPaginationOptions paging)
        {
            var entityType = typeof(T);

            var propertyInfo = entityType.GetProperty(paging.CursorField)
                ?? throw new InvalidOperationException(
                    $"Cursor field '{paging.CursorField}' not found on type '{entityType.Name}'.");

            var cursorValue = CursorEncryption.DecodeCursor(
                paging.Cursor!,
                propertyInfo.PropertyType);

            var parameter = Expression.Parameter(entityType, "e");

            // e.CursorField
            var propertyAccess = Expression.Property(parameter, propertyInfo);

            // constant(cursorValue) with correct type
            var constant = Expression.Constant(cursorValue, propertyInfo.PropertyType);

            // e.CursorField > cursorValue  (or <)
            var comparison = paging.Direction == CursorDirection.Forward
                ? Expression.GreaterThan(propertyAccess, constant)
                : Expression.LessThan(propertyAccess, constant);

            return source.Where(Expression.Lambda<Func<T, bool>>(comparison, parameter));
        }
    }
}
