using Sumapap.Queries.Execution.Evaluators;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Enumerable
{
    internal static class EnumerableSorting
    {
        public static IEnumerable<T> Apply<T>(
            IEnumerable<T> source,
            SortOptions? sort)
        {
            if (sort == null || sort.Sorts.Count == 0)
                return source;

            IOrderedEnumerable<T>? ordered = null;

            foreach (var descriptor in sort.Sorts)
            {
                SortEvaluator.EvaluateDescriptor(descriptor, source, ref ordered);
            }

            return ordered ?? source;
        }
    }
}
