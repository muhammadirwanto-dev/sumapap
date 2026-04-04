using System.Collections;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Evaluators;
using Sumapap.Queries.Execution.Utils;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Executors
{
    public sealed class EnumerableQueryExecutor<T>
        : QueryExecutorBase<IEnumerable<T>, T>
    {
        public override IQueryResult<T> Execute(IQuery query, IEnumerable<T> source)
        {
            var filtered = ApplyFiltering(source, query);
            var sorted = ApplySorting(filtered, query);

            return ApplyPaging(sorted, query);
        }

        public override Task<IQueryResult<T>> ExecuteAsync(IQuery query, IEnumerable<T> source, CancellationToken cancellationToken = default)
            => Task.FromResult(Execute(query, source));

        protected override IEnumerable<T> ApplyFiltering(IEnumerable<T> source, IQuery query)
            => source.Where(item => FilterEvaluator.EvaluateGroup(query.Filters.RootGroup, item));

        protected override IQueryResult<T> ApplyPaging(IEnumerable<T> source, IQuery query)
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
                    .Take(page.PageSize);

                return new QueryResult<T>(items, total, new PageInfo(
                    hasNextPage: page.Offset + page.PageSize < total,
                    hasPreviousPage: page.Offset > 0)
                    );
            }

            return new QueryResult<T>([.. source], total);
        }


        protected override IQueryResult<T> ApplyCursorPaging(IEnumerable<T> source, IQuery query)
        {
            var paging = query.CursorPaging!;
            var prop = ReflectionCache.GetProperty<T>(paging.CursorField)
                ?? throw new InvalidOperationException(
                    $"Cursor field '{paging.CursorField}' not found on '{typeof(T).Name}'.");

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                var cursorValue = CursorEncryption.DecodeCursor(
                    paging.Cursor,
                    prop.PropertyType);

                source = paging.Direction == CursorDirection.Forward
                    ? source.Where(x =>
                        Comparer.Default.Compare(prop.GetValue(x), cursorValue) > 0)
                    : source.Where(x =>
                        Comparer.Default.Compare(prop.GetValue(x), cursorValue) < 0);
            }

            var items = source.Take(paging.Limit + 1).ToList();
            var hasNext = items.Count > paging.Limit;
            var result = items.Take(paging.Limit).ToList();

            var endCursor = result.Count > 0
                ? CursorEncryption.EncodeCursor(
                    prop.GetValue(result.Last())!)
                : null;

            return new QueryResult<T>(
                result,
                totalDataCount: -1,
                pageInfo: new PageInfo(
                    hasNextPage: hasNext,
                    hasPreviousPage: paging.Cursor != null,
                    startCursor: paging.Cursor,
                    endCursor: endCursor
                )
            );
        }

        protected override IEnumerable<T> ApplySorting(IEnumerable<T> source, IQuery query)
        {
            SortOptions sort = query.Sort;

            if (sort.Sorts.Count == 0)
                return source;

            IOrderedEnumerable<T>? ordered = null;

            foreach (var descriptor in sort.Sorts)
            {
                SortEvaluator.EvaluateDescriptor(descriptor, source, ref ordered);
            }

            return ordered ?? source;
        }
    }
}
