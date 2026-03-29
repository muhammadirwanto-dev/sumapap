using Sumapap.Queries.Execution.Abstraction;

namespace Sumapap.Queries.Execution.Enumerable
{
    public sealed class EnumerableQueryExecutor<T>
        : IQueryExecutor<IEnumerable<T>, T>
    {
        public QueryResult<T> Execute(Query query, IEnumerable<T> source)
        {
            var filtered = EnumerableFiltering.Apply(source, query.Filters);
            var sorted = EnumerableSorting.Apply(filtered, query.Sort);

            return EnumerablePaging.Apply(sorted, query);
        }

        public Task<QueryResult<T>> ExecuteAsync(Query query, IEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(query, source));
        }
    }
}
