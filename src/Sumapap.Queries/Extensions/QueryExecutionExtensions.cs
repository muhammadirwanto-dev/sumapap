using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Factories;

namespace Sumapap.Queries.Extensions
{
    public static class QueryExecutionExtensions
    {
        public static IQueryResult<T> Execute<T>(this IQuery query, IEnumerable<T> source)
            => ExecutorFactory.Instance.Create<IEnumerable<T>, T>().Execute(query, source);

        public static Task<IQueryResult<T>> ExecuteAsync<T>(
            this IQuery query,
            IEnumerable<T> source,
            CancellationToken cancellationToken = default)
            => ExecutorFactory.Instance.Create<IEnumerable<T>, T>().ExecuteAsync(query, source, cancellationToken);

        public static IQueryResult<T> Execute<T>(this IQuery query, IQueryable<T> source)
            => ExecutorFactory.Instance.Create<IQueryable<T>, T>().Execute(query, source);

        public static Task<IQueryResult<T>> ExecuteAsync<T>(
            this IQuery query,
            IQueryable<T> source,
            CancellationToken cancellationToken = default)
            => ExecutorFactory.Instance.Create<IQueryable<T>, T>().ExecuteAsync(query, source, cancellationToken);
    }
}
