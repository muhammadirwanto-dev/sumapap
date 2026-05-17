using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Abstraction;
using Sumapap.Queries.Execution.Utils;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.Executors
{
    public abstract class QueryExecutorBase<TSource, TResult>
        : IQueryExecutor<TSource, TResult>
        where TSource : IEnumerable<TResult>
    {
        protected readonly ExpressionBuilder<TResult> _expressionBuilder = new();

        public IQueryResult<TResult> Execute(IQuery query, TSource source)
        {
            var filtered = ApplyFiltering(source, query);
            var sorted = ApplySorting(filtered, query);

            return ApplyPaging(sorted, query);
        }

        public Task<IQueryResult<TResult>> ExecuteAsync(IQuery query, TSource source, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(query, source));
        }

        protected abstract TSource ApplyFiltering(TSource source, IQuery query);

        protected abstract TSource ApplySorting(TSource source, IQuery query);

        protected IQueryResult<TResult> ApplyPaging(TSource source, IQuery query)
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

                return new QueryResult<TResult>(
                    items,
                    total,
                    new PageInfo(
                        hasNextPage: page.Offset + page.PageSize < total,
                        hasPreviousPage: page.Offset > 0)
                    );
            }

            return new QueryResult<TResult>(source, total);
        }

        protected abstract IQueryResult<TResult> ApplyCursorPaging(TSource source, IQuery query);
    }
}
