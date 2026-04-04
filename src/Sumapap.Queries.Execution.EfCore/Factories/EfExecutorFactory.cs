using Sumapap.Queries.Execution.Abstraction;
using Sumapap.Queries.Execution.EfCore.Executors;

namespace Sumapap.Queries.Execution.EfCore.Factories
{
    public class EfExecutorFactory
    {
        public static readonly EfExecutorFactory Default = new();

#pragma warning disable CA1822 // Mark members as static
        public IQueryExecutor<TSource, TResult> Create<TSource, TResult>()
#pragma warning restore CA1822 // Mark members as static
            where TSource : IEnumerable<TResult>
        {
            if (typeof(IQueryable<TResult>).IsAssignableFrom(typeof(TSource)))
            {
                return (IQueryExecutor<TSource, TResult>)(object)new EfQueryableQueryExecutor<TResult>();
            }

            throw new NotSupportedException($"Type {typeof(TSource)} is not supported.");
        }

    }
}
