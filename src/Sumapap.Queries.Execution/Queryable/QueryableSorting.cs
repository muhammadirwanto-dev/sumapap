using Sumapap.Queries.Execution.Evaluators;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryableSorting
    {
        public static IQueryable<T> Apply<T>(
            IQueryable<T> source,
            SortOptions sort)
        {
            if (sort == null || sort.Sorts.Count == 0)
                return source;

            IOrderedQueryable<T>? ordered = null;

            foreach (var descriptor in sort.Sorts)
            {
                SortEvaluator.EvaluateDescriptor(descriptor, source, ref ordered);
            }

            return ordered ?? source;
        }
    }
}
