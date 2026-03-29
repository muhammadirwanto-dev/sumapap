using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Enumerable
{
    internal static class EnumerableFiltering
    {
        public static IEnumerable<T> Apply<T>(
            IEnumerable<T> source,
            FilterOptions filters)
        {
            foreach (var filter in filters.Filters)
            {
                var prop = ReflectionCache.GetProperty<T>(filter.Field);
                if (prop == null)
                    continue;

                source = source.Where(item =>
                {
                    var value = prop.GetValue(item);
                    return FilterEvaluator.Evaluate(
                        value,
                        filter.Value,
                        filter.Operator);
                });
            }

            return source;
        }
    }
}
