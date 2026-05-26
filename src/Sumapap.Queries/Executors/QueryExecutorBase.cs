using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Paging;
using Sumapap.Queries.Utils;

namespace Sumapap.Queries.Executors
{
    /// <summary>
    /// Base class for query executors that provides common pagination logic.
    /// </summary>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of items in the result set.</typeparam>
    public abstract class QueryExecutorBase<TSource, TResult>
        : IQueryExecutor<TSource, TResult>
        where TSource : IEnumerable<TResult>
    {
        /// <summary>
        /// Expression builder for creating filter and sort expressions.
        /// </summary>
        protected readonly ExpressionBuilder<TResult> _expressionBuilder = new();

        /// <summary>
        /// Executes the query synchronously against the specified data source.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="source">The data source to query.</param>
        /// <returns>The query result containing items and pagination metadata.</returns>
        public IQueryResult<TResult> Execute(IQuery query, TSource source)
        {
            var filtered = ApplyFiltering(source, query);
            var sorted = ApplySorting(filtered, query);

            return ApplyPaging(sorted, query);
        }

        /// <summary>
        /// Executes the query asynchronously against the specified data source.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="source">The data source to query.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the query result.</returns>
        public Task<IQueryResult<TResult>> ExecuteAsync(IQuery query, TSource source, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(query, source));
        }

        /// <summary>
        /// Applies filtering to the data source based on the query's filter configuration.
        /// </summary>
        /// <param name="source">The data source.</param>
        /// <param name="query">The query containing filter criteria.</param>
        /// <returns>The filtered data source.</returns>
        protected abstract TSource ApplyFiltering(TSource source, IQuery query);

        /// <summary>
        /// Applies sorting to the data source based on the query's sort configuration.
        /// </summary>
        /// <param name="source">The data source.</param>
        /// <param name="query">The query containing sort criteria.</param>
        /// <returns>The sorted data source.</returns>
        protected abstract TSource ApplySorting(TSource source, IQuery query);

        /// <summary>
        /// Applies pagination to the data source and returns the paginated result.
        /// </summary>
        /// <param name="source">The data source.</param>
        /// <param name="query">The query containing pagination configuration.</param>
        /// <returns>The paginated query result.</returns>
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

        /// <summary>
        /// Applies cursor-based pagination to the data source.
        /// </summary>
        /// <param name="source">The data source.</param>
        /// <param name="query">The query containing cursor pagination configuration.</param>
        /// <returns>The cursor-paginated query result.</returns>
        protected abstract IQueryResult<TResult> ApplyCursorPaging(TSource source, IQuery query);
    }
}
