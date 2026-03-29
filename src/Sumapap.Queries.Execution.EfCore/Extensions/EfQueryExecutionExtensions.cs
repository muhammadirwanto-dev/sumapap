using Sumapap.Queries.Execution.EfCore.Queryable;

namespace Sumapap.Queries.Execution.EfCore.Extensions
{
    public static class EfQueryExecutionExtensions
    {
        extension(Query query)
        {
            public QueryResult<T> Execute<T>(
                IQueryable<T> source)
                => new EfQueryableQueryExecutor<T>().Execute(query, source);

            public Task<QueryResult<T>> ExecuteAsync<T>(
                IQueryable<T> source,
                CancellationToken cancellationToken = default)
                => new EfQueryableQueryExecutor<T>().ExecuteAsync(query, source, cancellationToken);
        }
    }
}
