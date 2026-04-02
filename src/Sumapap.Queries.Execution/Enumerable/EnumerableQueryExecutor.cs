using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Abstraction;

namespace Sumapap.Queries.Execution.Enumerable
{
    public sealed class EnumerableQueryExecutor<T>
        : IQueryExecutor<IEnumerable<T>, T>
    {
        public IQueryResult<T> Execute(IQuery query, IEnumerable<T> source)
        {
            var filtered = EnumerableFiltering.Apply(source, query.Filters);
            var sorted = EnumerableSorting.Apply(filtered, query.Sort);

            return EnumerablePaging.Apply(sorted, query);
        }

        public Task<IQueryResult<T>> ExecuteAsync(IQuery query, IEnumerable<T> source, CancellationToken cancellationToken = default)
            => Task.FromResult(Execute(query, source));
    }
}
