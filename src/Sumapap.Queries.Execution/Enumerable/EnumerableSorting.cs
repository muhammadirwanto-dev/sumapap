using Sumapap.Queries.Execution.Internals;
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

            foreach (var s in sort.Sorts)
            {
                var prop = ReflectionCache.GetProperty<T>(s.Field);
                if (prop == null)
                    continue;

                ordered = ordered == null
                    ? s.Direction == SortDirection.Asc
                        ? source.OrderBy(x => prop.GetValue(x))
                        : source.OrderByDescending(x => prop.GetValue(x))
                    : s.Direction == SortDirection.Asc
                        ? ordered.ThenBy(x => prop.GetValue(x))
                        : ordered.ThenByDescending(x => prop.GetValue(x));
            }

            return ordered ?? source;
        }
    }
}
