using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Factories;

namespace Sumapap.Queries.Execution.Extensions
{
    public static class QueryExecutionExtensions
    {
        extension(IQuery query)
        {
            public IQueryResult<T> Execute<T>(IEnumerable<T> source)
                => ExecutorFactory.Instance.Create<IEnumerable<T>, T>().Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IEnumerable<T> source,
                CancellationToken cancellationToken = default)
                => ExecutorFactory.Instance.Create<IEnumerable<T>, T>().ExecuteAsync(query, source, cancellationToken);

            public IQueryResult<T> Execute<T>(IQueryable<T> source)
                => ExecutorFactory.Instance.Create<IQueryable<T>, T>().Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IQueryable<T> source,
                CancellationToken cancellationToken = default)
                => ExecutorFactory.Instance.Create<IQueryable<T>, T>().ExecuteAsync(query, source, cancellationToken);
        }
    }
}
