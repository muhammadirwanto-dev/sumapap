using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryableSorting
    {
        public static IQueryable<T> Apply<T>(
            IQueryable<T> source,
            SortOptions sort)
        {
            if (sort.Sorts.Count == 0)
                return source;

            IOrderedQueryable<T>? ordered = null;

            foreach (var s in sort.Sorts)
            {
                ordered = ExpressionCache.ApplyOrdering(
                    ordered ?? source,
                    s);
            }

            return ordered ?? source;
        }
    }
}
