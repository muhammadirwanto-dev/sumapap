using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Queries.Execution.Abstraction;
using Sumapap.Queries.Execution.Cursor;
using Sumapap.Queries.Execution.EfCore.Expressions;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.EfCore.Queryable
{
    public class EfQueryableQueryExecutor<T> : IQueryExecutor<IQueryable<T>, T>
    {
        public QueryResult<T> Execute(Query query, IQueryable<T> source)
        {
            source = ApplyFiltering(source, query.Filters);
            source = ApplySorting(source, query.Sort);

            return ApplyPaging(source, query);
        }

        public Task<QueryResult<T>> ExecuteAsync(Query query, IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            source = ApplyFiltering(source, query.Filters);
            source = ApplySorting(source, query.Sort);

            return ApplyPagingAsync(source, query, cancellationToken);
        }

        private static IQueryable<T> ApplyFiltering(IQueryable<T> source, FilterOptions filters)
        {
            foreach (var filter in filters.Filters)
            {
                source = filter.Operator switch
                {
                    FilterOperator.Equals =>
                        source.Where(ComparisonExpression.Build<T>(
                            filter.Field, filter.Value, ExpressionType.Equal)),

                    FilterOperator.NotEquals =>
                        source.Where(ComparisonExpression.Build<T>(
                            filter.Field, filter.Value, ExpressionType.NotEqual)),

                    FilterOperator.GreaterThan =>
                        source.Where(ComparisonExpression.Build<T>(
                            filter.Field, filter.Value, ExpressionType.GreaterThan)),

                    FilterOperator.GreaterThanOrEqual =>
                        source.Where(ComparisonExpression.Build<T>(
                            filter.Field, filter.Value, ExpressionType.GreaterThanOrEqual)),

                    FilterOperator.LessThan =>
                        source.Where(ComparisonExpression.Build<T>(
                            filter.Field, filter.Value, ExpressionType.LessThan)),

                    FilterOperator.LessThanOrEqual =>
                        source.Where(ComparisonExpression.Build<T>(
                            filter.Field, filter.Value, ExpressionType.LessThanOrEqual)),

                    FilterOperator.Contains =>
                        source.Where(StringMethodExpression.Build<T>(
                            filter.Field, filter.Value, nameof(string.Contains))),

                    FilterOperator.StartsWith =>
                        source.Where(StringMethodExpression.Build<T>(
                            filter.Field, filter.Value, nameof(string.StartsWith))),

                    FilterOperator.EndsWith =>
                        source.Where(StringMethodExpression.Build<T>(
                            filter.Field, filter.Value, nameof(string.EndsWith))),

                    FilterOperator.In =>
                        source.Where(InExpression.Build<T>(
                            filter.Field, filter.Value)),

                    _ => source
                };
            }

            return source;
        }

        private static IQueryable<T> ApplySorting(IQueryable<T> source, SortOptions? sort)
        {
            if (sort == null || sort.Sorts.Count == 0)
            {
                return source;
            }

            IOrderedQueryable<T>? ordered = null;

            foreach (var descriptor in sort.Sorts)
            {
                ordered = ordered == null
                    ? descriptor.Direction == SortDirection.Asc
                        ? source.OrderBy(e => EF.Property<object>(e!, descriptor.Field))
                        : source.OrderByDescending(e => EF.Property<object>(e!, descriptor.Field))
                    : descriptor.Direction == SortDirection.Asc
                        ? ordered.ThenBy(e => EF.Property<object>(e!, descriptor.Field))
                        : ordered.ThenByDescending(e => EF.Property<object>(e!, descriptor.Field));
            }

            return ordered ?? source;
        }

        private static QueryResult<T> ApplyPaging(
            IQueryable<T> source,
            Query query)
        {
            if (query.UsesCursorPaging)
            {
                var paging = query.CursorPaging!;

                // Ensure deterministic ordering (required for cursor paging)
                source = ApplyCursorOrdering(source, paging);

                if (!string.IsNullOrEmpty(paging.Cursor))
                {
                    source = ApplyCursorFilter(source, paging);
                }

                // Fetch limit + 1 to detect next page
                var items = source
                    .Take(paging.Limit + 1)
                    .ToList();

                var hasNext = items.Count > paging.Limit;
                var result = items.Take(paging.Limit).ToList();

                var endCursor = result.Count > 0
                    ? CursorEncryption.EncodeCursor(
                        GetPropertyValue(result.Last(), paging.CursorField)!)
                    : null;

                return new QueryResult<T>(
                    result,
                    totalDataCount: -1, // 🚨 cursor paging never exposes total count
                    pageInfo: new PageInfo(
                        hasNextPage: hasNext,
                        hasPreviousPage: paging.Cursor != null,
                        startCursor: paging.Cursor,
                        endCursor: endCursor
                    )
                );
            }

            // Offset paging fallback
            var totalCount = source.Count();

            if (query.UsesOffsetPaging)
            {
                var paged = source
                    .Skip(query.OffsetPaging!.Offset)
                    .Take(query.OffsetPaging!.PageSize)
                    .ToList();

                return new QueryResult<T>(paged, totalCount);
            }

            return new QueryResult<T>([.. source], totalCount);
        }

        private static async Task<QueryResult<T>> ApplyPagingAsync(
            IQueryable<T> source,
            Query query,
            CancellationToken cancellationToken = default)
        {
            if (query.UsesCursorPaging)
            {
                var paging = query.CursorPaging!;

                // Ensure deterministic ordering (required for cursor paging)
                source = ApplyCursorOrdering(source, paging);

                if (!string.IsNullOrEmpty(paging.Cursor))
                {
                    source = ApplyCursorFilter(source, paging);
                }

                // Fetch limit + 1 to detect next page
                var items = await source
                    .Take(paging.Limit + 1)
                    .ToListAsync(cancellationToken);

                var hasNext = items.Count > paging.Limit;
                var result = items.Take(paging.Limit).ToList();

                var endCursor = result.Count > 0
                    ? CursorEncryption.EncodeCursor(
                        GetPropertyValue(result.Last(), paging.CursorField)!)
                    : null;

                return new QueryResult<T>(
                    result,
                    totalDataCount: -1, // 🚨 cursor paging never exposes total count
                    pageInfo: new PageInfo(
                        hasNextPage: hasNext,
                        hasPreviousPage: paging.Cursor != null,
                        startCursor: paging.Cursor,
                        endCursor: endCursor
                    )
                );
            }

            // Offset paging fallback
            var totalCount = await source.CountAsync(cancellationToken);

            if (query.UsesOffsetPaging)
            {
                var paged = await source
                    .Skip(query.OffsetPaging!.Offset)
                    .Take(query.OffsetPaging!.PageSize)
                    .ToListAsync(cancellationToken);

                return new QueryResult<T>(paged, totalCount);
            }

            var all = await source.ToListAsync(cancellationToken);
            return new QueryResult<T>(all, totalCount);
        }

        private static IQueryable<T> ApplyCursorOrdering(
            IQueryable<T> source,
            CursorPaginationOptions paging)
        {
            return paging.Direction == CursorDirection.Forward
                ? source.OrderBy(e => EF.Property<object>(e!, paging.CursorField))
                : source.OrderByDescending(e => EF.Property<object>(e!, paging.CursorField));
        }

        private static IQueryable<T> ApplyCursorFilter(
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

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            return source.Where(lambda);
        }

        private static object? GetPropertyValue(T entity, string field)
        {
            return typeof(T)
                .GetProperty(field)?
                .GetValue(entity);
        }
    }
}
