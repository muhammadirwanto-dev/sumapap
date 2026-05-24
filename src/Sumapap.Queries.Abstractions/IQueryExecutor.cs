namespace Sumapap.Queries.Abstractions
{
    public interface IQueryExecutor<TSource, TResult>
    {
        IQueryResult<TResult> Execute(IQuery query, TSource source);

        Task<IQueryResult<TResult>> ExecuteAsync(IQuery query, TSource source, CancellationToken cancellationToken = default);
    }
}
