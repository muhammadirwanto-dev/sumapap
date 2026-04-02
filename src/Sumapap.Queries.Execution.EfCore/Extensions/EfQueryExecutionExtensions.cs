using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.EfCore.Queryable;

namespace Sumapap.Queries.Execution.EfCore.Extensions
{
    public static class EfQueryExecutionExtensions
    {
        private static class Cache<T>
        {
            internal static readonly EfQueryableQueryExecutor<T> EfQueryableExecutor = new();
        }

        extension(IQuery query)
        {
            public IQueryResult<T> Execute<T>(
                IQueryable<T> source)
                => Cache<T>.EfQueryableExecutor.Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IQueryable<T> source,
                CancellationToken cancellationToken = default)
                => Cache<T>.EfQueryableExecutor.ExecuteAsync(query, source, cancellationToken);
        }
    }
}
