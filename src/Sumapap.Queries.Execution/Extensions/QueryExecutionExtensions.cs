using Sumapap.Queries.Execution.Enumerable;
using Sumapap.Queries.Execution.Queryable;

namespace Sumapap.Queries.Execution.Extensions
{
    public static class QueryExecutionExtensions
    {
        public static QueryResult<T> Execute<T>(
            this Query query,
            IEnumerable<T> source)
            => new EnumerableQueryExecutor<T>().Execute(query, source);

        public static Task<QueryResult<T>> ExecuteAsync<T>(
            this Query query,
            IEnumerable<T> source,
            CancellationToken cancellationToken = default)
            => new EnumerableQueryExecutor<T>().ExecuteAsync(query, source, cancellationToken);

        public static QueryResult<T> Execute<T>(
            this Query query,
            IQueryable<T> source)
            => new QueryableQueryExecutor<T>().Execute(query, source);

        public static Task<QueryResult<T>> ExecuteAsync<T>(
            this Query query,
            IQueryable<T> source,
            CancellationToken cancellationToken = default)
            => new QueryableQueryExecutor<T>().ExecuteAsync(query, source, cancellationToken);
    }
}
