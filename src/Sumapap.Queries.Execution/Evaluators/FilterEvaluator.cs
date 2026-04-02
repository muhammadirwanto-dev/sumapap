using System.Globalization;
using Sumapap.Queries.Execution.Common;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Evaluators
{
    public static class FilterEvaluator
    {
        public static bool EvaluateGroup<T>(FilterGroup group, T item)
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
                bool match = EvaluateDescriptor(filter, item);
                result = Combine(result, match, group.Operator);

                if (isAnd && !result)
                    return false;
                if (!isAnd && result)
                    return true;
            }

            foreach (var subGroup in group.SubGroups)
            {
                bool match = EvaluateGroup(subGroup, item);
                result = Combine(result, match, group.Operator);

                if (isAnd && !result)
                    return false;
                if (!isAnd && result)
                    return true;
            }

            return result;
        }

        public static bool EvaluateDescriptor<T>(FilterDescriptor descriptor, T item)
        {
            var prop = ReflectionCache.GetProperty<T>(descriptor.Field);
            if (prop == null)
            {
                return false;
            }

            var value = prop.GetValue(item);

            return EvaluateDescriptor(descriptor, value);
        }

        public static bool Evaluate(FilterDescriptor descriptor, object? value)
            => descriptor.Operator switch
            {
                FilterOperator.Equals =>
                    Equals(value, descriptor.Value),

                FilterOperator.NotEquals =>
                    !Equals(value, descriptor.Value),

                FilterOperator.GreaterThan =>
                    Compare(value, descriptor.Value) > 0,

                FilterOperator.GreaterThanOrEqual =>
                    Compare(value, descriptor.Value) >= 0,

                FilterOperator.LessThan =>
                    Compare(value, descriptor.Value) < 0,

                FilterOperator.LessThanOrEqual =>
                    Compare(value, descriptor.Value) <= 0,

                FilterOperator.Contains =>
                    StringMatch(value, descriptor.Value, (a, b) =>
                        a.Contains(b, StringComparison.OrdinalIgnoreCase)),

                FilterOperator.StartsWith =>
                    StringMatch(value, descriptor.Value, (a, b) =>
                        a.StartsWith(b, StringComparison.OrdinalIgnoreCase)),

                FilterOperator.EndsWith =>
                    StringMatch(value, descriptor.Value, (a, b) =>
                        a.EndsWith(b, StringComparison.OrdinalIgnoreCase)),

                FilterOperator.In =>
                    descriptor.Value is IEnumerable<object?> values &&
                    values.Any(v => Equals(value, v)),

                _ => true
            };

        private static bool Combine(bool current, bool next, CompositeOperator op)
            => op == CompositeOperator.And ? current && next : current || next;

        private static int Compare(object? left, object? right)
        {
            if (left == null || right == null)
            {
                return -1;
            }

            if (left is IComparable comparable)
            {
                var converted = Convert.ChangeType(
                    right,
                    left.GetType(),
                    CultureInfo.InvariantCulture);

                return comparable.CompareTo(converted);
            }

            return -1;
        }

        private static bool StringMatch(
            object? value,
            object? filterValue,
            Func<string, string, bool> matcher)
            => value != null && filterValue != null && matcher(value.ToString()!, filterValue.ToString()!);
    }
}
