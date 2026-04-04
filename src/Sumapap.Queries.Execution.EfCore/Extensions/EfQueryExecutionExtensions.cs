using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.EfCore.Factories;

namespace Sumapap.Queries.Execution.EfCore.Extensions
{
    public static class EfQueryExecutionExtensions
    {
        extension(IQuery query)
        {
            public IQueryResult<T> Execute<T>(
                IQueryable<T> source)
                => EfExecutorFactory.Default.Create<IQueryable<T>, T>().Execute(query, source);

            public Task<IQueryResult<T>> ExecuteAsync<T>(
                IQueryable<T> source,
                CancellationToken cancellationToken = default)
                => EfExecutorFactory.Default.Create<IQueryable<T>, T>().ExecuteAsync(query, source, cancellationToken);
        }
    }
}
