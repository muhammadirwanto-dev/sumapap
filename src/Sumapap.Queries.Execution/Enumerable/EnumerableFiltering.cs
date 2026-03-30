using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Enumerable
{
    internal static class EnumerableFiltering
    {
        public static IEnumerable<T> Apply<T>(IEnumerable<T> source, FilterOptions options)
        {
            return options?.RootGroup == null
                ? source
                : source.Where(item => EvaluateGroup(item, options.RootGroup));
        }

        private static bool EvaluateGroup<T>(T item, FilterGroup group)
        {
            // For AND: All must be true (Default for empty groups: true)
            // For OR:  Any must be true (Default for empty groups: false)
            bool isAnd = group.Operator == CompositeOperator.And;

            // Short-circuiting logic:
            // If AND, we start true and look for a fail.
            // If OR, we start false and look for a win.
            bool result = isAnd;

            foreach (var filter in group.Filters)
            {
                bool match = EvaluateFilter(item, filter);
                result = Combine(result, match, group.Operator);

                if (isAnd && !result)
                    return false;
                if (!isAnd && result)
                    return true;
            }

            foreach (var subGroup in group.SubGroups)
            {
                bool match = EvaluateGroup(item, subGroup);
                result = Combine(result, match, group.Operator);

                if (isAnd && !result)
                    return false;
                if (!isAnd && result)
                    return true;
            }

            return result;
        }

        private static bool EvaluateFilter<T>(T item, FilterDescriptor filter)
        {
            var prop = ReflectionCache.GetProperty<T>(filter.Field);
            if (prop == null)
            {
                return false;
            }

            var value = prop.GetValue(item);

            return FilterEvaluator.Evaluate(value, filter.Value, filter.Operator);
        }

        private static bool Combine(bool current, bool next, CompositeOperator op)
            => op == CompositeOperator.And ? current && next : current || next;
    }
}
