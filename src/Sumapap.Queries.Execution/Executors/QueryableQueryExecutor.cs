using System.Linq.Expressions;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Evaluators;
using Sumapap.Queries.Execution.Utils;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Executors
{
    public sealed class QueryableQueryExecutor<T>
        : QueryExecutorBase<IQueryable<T>, T>
    {
        public override IQueryResult<T> Execute(IQuery query, IQueryable<T> source)
        {
            var filtered = ApplyFiltering(source, query);
            var sorted = ApplySorting(filtered, query);

            return ApplyPaging(sorted, query);
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

        protected override IQueryable<T> ApplyFiltering(IQueryable<T> source, IQuery query)
            => source.Where(item => FilterEvaluator.EvaluateGroup(query.Filters.RootGroup, item));

        protected override IQueryResult<T> ApplyPaging(IQueryable<T> source, IQuery query)
        {
            if (query.UsesCursorPaging)
            {
                return ApplyCursorPaging(source, query);
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

        protected override IQueryable<T> ApplySorting(IQueryable<T> source, IQuery query)
        {
            SortOptions sort = query.Sort;

            if (sort == null || sort.Sorts.Count == 0)
                return source;

            IOrderedQueryable<T>? ordered = null;

            foreach (var descriptor in sort.Sorts)
            {
                SortEvaluator.EvaluateDescriptor(descriptor, source, ref ordered);
            }

            return ordered ?? source;
        }

        private static IQueryable<T> ApplyCursorOrdering(
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

        private static IQueryable<T> ApplyCursorFiltering(
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
