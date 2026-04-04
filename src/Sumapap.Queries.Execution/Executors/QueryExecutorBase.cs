using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Abstraction;

namespace Sumapap.Queries.Execution.Executors
{
    public abstract class QueryExecutorBase<TSource, TResult>
        : IQueryExecutor<TSource, TResult>
    {
        public abstract IQueryResult<TResult> Execute(IQuery query, TSource source);

        public abstract Task<IQueryResult<TResult>> ExecuteAsync(IQuery query, TSource source, CancellationToken cancellationToken = default);

        protected abstract TSource ApplyFiltering(TSource source, IQuery query);

        protected abstract TSource ApplySorting(TSource source, IQuery query);

        protected abstract IQueryResult<TResult> ApplyPaging(TSource source, IQuery query);

        protected abstract IQueryResult<TResult> ApplyCursorPaging(TSource source, IQuery query);
    }
}
