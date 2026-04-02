using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Enumerable;
using Sumapap.Queries.Execution.Queryable;

namespace Sumapap.Queries.Execution.Extensions
{
    public static class QueryExecutionExtensions
    {
        private static class Cache<T>
        {
            internal static readonly EnumerableQueryExecutor<T> EnumerableExecutor = new();
            internal static readonly QueryableQueryExecutor<T> QueryableExecutor = new();
        }

        extension(IQuery query)
        {
            public IQueryResult<T> Execute<T>(IEnumerable<T> source)
                => Cache<T>.EnumerableExecutor.Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IEnumerable<T> source,
                CancellationToken cancellationToken = default)
                => Cache<T>.EnumerableExecutor.ExecuteAsync(query, source, cancellationToken);

            public IQueryResult<T> Execute<T>(IQueryable<T> source)
                => Cache<T>.QueryableExecutor.Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IQueryable<T> source,
                CancellationToken cancellationToken = default)
                => Cache<T>.QueryableExecutor.ExecuteAsync(query, source, cancellationToken);
        }
    }
}
