using Sumapap.Queries.Execution.Evaluators;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryableFiltering
    {
        public static IQueryable<T> Apply<T>(IQueryable<T> source, FilterOptions options)
        {
            return options?.RootGroup == null
                ? source
                : source.Where(item => FilterEvaluator.EvaluateGroup(options.RootGroup, item));
        }
    }
}
