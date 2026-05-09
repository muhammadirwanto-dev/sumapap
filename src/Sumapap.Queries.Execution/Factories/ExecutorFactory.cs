using Sumapap.Queries.Execution.Abstraction;
using Sumapap.Queries.Execution.Executors;

namespace Sumapap.Queries.Execution.Factories
{
    public class ExecutorFactory
    {
        public static readonly ExecutorFactory Instance = new();

        private ExecutorFactory() { }

#pragma warning disable CA1822 // Mark members as static
        public IQueryExecutor<TSource, TResult> Create<TSource, TResult>()
#pragma warning restore CA1822 // Mark members as static
            where TSource : IEnumerable<TResult>
        {
            if (typeof(IQueryable<TResult>).IsAssignableFrom(typeof(TSource)))
            {
                return (IQueryExecutor<TSource, TResult>)(object)new QueryableQueryExecutor<TResult>();
            }

            if (typeof(IEnumerable<TResult>).IsAssignableFrom(typeof(TSource)))
            {
                return (IQueryExecutor<TSource, TResult>)(object)new EnumerableQueryExecutor<TResult>();
            }

            throw new NotSupportedException($"Type {typeof(TSource)} is not supported.");
        }
    }
}
