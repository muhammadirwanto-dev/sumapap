namespace Sumapap.Queries.Abstractions
{
    /// <summary>
    /// Executes queries against a data source and returns paginated results.
    /// </summary>
    /// <typeparam name="TSource">The type of the data source (e.g., IQueryable&lt;T&gt; or IEnumerable&lt;T&gt;).</typeparam>
    /// <typeparam name="TResult">The type of items in the result set.</typeparam>
    public interface IQueryExecutor<TSource, TResult>
    {
        /// <summary>
        /// Executes the query synchronously against the specified data source.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="source">The data source to query.</param>
        /// <returns>The query result containing items and pagination metadata.</returns>
        IQueryResult<TResult> Execute(IQuery query, TSource source);

        /// <summary>
        /// Executes the query asynchronously against the specified data source.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="source">The data source to query.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the query result.</returns>
        Task<IQueryResult<TResult>> ExecuteAsync(IQuery query, TSource source, CancellationToken cancellationToken = default);
    }
}
