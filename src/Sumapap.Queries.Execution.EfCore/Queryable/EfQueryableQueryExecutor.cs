using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Execution.Abstraction;

namespace Sumapap.Queries.Execution.EfCore.Queryable
{
    public class EfQueryableQueryExecutor<T> : IQueryExecutor<IQueryable<T>, T>
    {
        public IQueryResult<T> Execute(IQuery query, IQueryable<T> source)
        {
            source = EfQueryableFiltering.Apply(source, query.Filters);
            source = EfQueryableSorting.Apply(source, query.Sort);

            return EfQueryablePaging.Apply(source, query);
        }

        public Task<IQueryResult<T>> ExecuteAsync(IQuery query, IQueryable<T> source, CancellationToken cancellationToken = default)
            => Task.FromResult(Execute(query, source));
    }
}
