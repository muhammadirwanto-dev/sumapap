using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Factories;

namespace Sumapap.Queries.Execution.Extensions
{
    public static class QueryExecutionExtensions
    {
        extension(IQuery query)
        {
            public IQueryResult<T> Execute<T>(IEnumerable<T> source)
                => ExecutorFactory.Default.Create<IEnumerable<T>, T>().Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IEnumerable<T> source,
                CancellationToken cancellationToken = default)
                => ExecutorFactory.Default.Create<IEnumerable<T>, T>().ExecuteAsync(query, source, cancellationToken);

            public IQueryResult<T> Execute<T>(IQueryable<T> source)
                => ExecutorFactory.Default.Create<IQueryable<T>, T>().Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IQueryable<T> source,
                CancellationToken cancellationToken = default)
                => ExecutorFactory.Default.Create<IQueryable<T>, T>().ExecuteAsync(query, source, cancellationToken);
        }
    }
}
