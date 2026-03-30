using Sumapap.Queries.Execution.Abstraction;

namespace Sumapap.Queries.Execution.Queryable
{
    public sealed class QueryableQueryExecutor<T>
        : IQueryExecutor<IQueryable<T>, T>
    {
        public QueryResult<T> Execute(Query query, IQueryable<T> source)
        {
            var filtered = QueryableFiltering.Apply(source, query.Filters);
            var sorted = QueryableSorting.Apply(filtered, query.Sort);

            return QueryablePaging.Apply(sorted, query);
        }

        public Task<QueryResult<T>> ExecuteAsync(Query query, IQueryable<T> source, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(query, source));
        }
    }
}
