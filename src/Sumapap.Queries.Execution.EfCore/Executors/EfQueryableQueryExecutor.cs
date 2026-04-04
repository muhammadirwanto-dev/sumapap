using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.EfCore.Expressions;
using Sumapap.Queries.Execution.Executors;
using Sumapap.Queries.Execution.Extensions;
using Sumapap.Queries.Execution.Utils;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.EfCore.Executors
{
    public class EfQueryableQueryExecutor<T>
        : QueryExecutorBase<IQueryable<T>, T>
    {
        public override IQueryResult<T> Execute(IQuery query, IQueryable<T> source)
        {
            source = ApplyFiltering(source, query);
            source = ApplySorting(source, query);

            return ApplyPaging(source, query);
        }

        public override Task<IQueryResult<T>> ExecuteAsync(IQuery query, IQueryable<T> source, CancellationToken cancellationToken = default)
            => Task.FromResult(Execute(query, source));

        protected override IQueryResult<T> ApplyCursorPaging(IQueryable<T> source, IQuery query)
        {
            var paging = query.CursorPaging!;

            source = ApplyCursorOrdering(source, paging);

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                source = ApplyCursorFiltering(source, paging);
            }

            var items = source.Take(paging.Limit + 1);
            var hasNext = items.Count() > paging.Limit;
            var result = items.Take(paging.Limit);

            var endCursor = result.Any()
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

        protected override IQueryable<T> ApplyFiltering(IQueryable<T> source, IQuery query)
        {
            // One parameter to rule them all: 'x'
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = BuildGroupExpression(query.Filters.RootGroup, parameter);

            if (body == null)
            {
                return source;
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(body, parameter));
        }

        protected override IQueryResult<T> ApplyPaging(IQueryable<T> source, IQuery query)
        {
            if (query.UsesCursorPaging)
            {
                return ApplyCursorPaging(source, query);
            }

            var total = source.Count();

            if (query.UsesOffsetPaging)
            {
                var paged = source
                    .Skip(query.OffsetPaging!.Offset)
                    .Take(query.OffsetPaging!.PageSize);

                return new QueryResult<T>(paged, total, new PageInfo(
                    hasNextPage: query.OffsetPaging.Offset + query.OffsetPaging.PageSize < total,
                    hasPreviousPage: query.OffsetPaging.Offset > 0)
                    );
            }

            return new QueryResult<T>([.. source], total);
        }

        protected override IQueryable<T> ApplySorting(IQueryable<T> source, IQuery query)
        {
            if (query.Sort.Sorts.Count == 0)
            {
                return source;
            }

            IOrderedQueryable<T>? ordered = null;

            foreach (var descriptor in query.Sort.Sorts)
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


        private static Expression? BuildGroupExpression(FilterGroup group, ParameterExpression parameter)
        {
            Expression? combined = null;

            foreach (var filter in group.Filters)
            {
                var leaf = BuildLeafExpression(filter, parameter);
                combined = combined.Combine(leaf, group.Operator);
            }

            foreach (var subGroup in group.SubGroups)
            {
                var subs = BuildGroupExpression(subGroup, parameter);
                combined = combined.Combine(subs, group.Operator);
            }

            return combined;
        }

        private static Expression BuildLeafExpression(FilterDescriptor filter, ParameterExpression parameter)
        {
            return filter.Operator switch
            {
                FilterOperator.Equals => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.Equal, parameter),
                FilterOperator.NotEquals => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.NotEqual, parameter),
                FilterOperator.GreaterThan => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.GreaterThan, parameter),
                FilterOperator.GreaterThanOrEqual => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.GreaterThanOrEqual, parameter),
                FilterOperator.LessThan => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.LessThan, parameter),
                FilterOperator.LessThanOrEqual => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.LessThanOrEqual, parameter),
                FilterOperator.Contains => StringMethodExpression.Build<T>(filter.Field, filter.Value, nameof(string.Contains), parameter),
                FilterOperator.StartsWith => StringMethodExpression.Build<T>(filter.Field, filter.Value, nameof(string.StartsWith), parameter),
                FilterOperator.EndsWith => StringMethodExpression.Build<T>(filter.Field, filter.Value, nameof(string.EndsWith), parameter),
                FilterOperator.In => InExpression.Build<T>(filter.Field, filter.Value, parameter),
                _ => Expression.Constant(true)
            };
        }

        private static IQueryable<T> ApplyCursorOrdering(
            IQueryable<T> source,
            CursorPaginationOptions paging)
        {
            return paging.Direction == CursorDirection.Forward
                ? source.OrderBy(e => EF.Property<object>(e!, paging.CursorField))
                : source.OrderByDescending(e => EF.Property<object>(e!, paging.CursorField));
        }

        private static IQueryable<T> ApplyCursorFiltering(
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
