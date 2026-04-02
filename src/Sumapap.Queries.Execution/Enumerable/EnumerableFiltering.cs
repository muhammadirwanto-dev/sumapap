using Sumapap.Queries.Execution.Evaluators;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Enumerable
{
    internal static class EnumerableFiltering
    {
        public static IEnumerable<T> Apply<T>(IEnumerable<T> source, FilterOptions options)
        {
            return options?.RootGroup == null
                ? source
                : source.Where(item => FilterEvaluator.EvaluateGroup(options.RootGroup, item));
        }
    }
}
