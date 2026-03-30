using System.Globalization;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Enumerable
{
    internal static class FilterEvaluator
    {
        public static bool Evaluate(
            object? value,
            object? filterValue,
            FilterOperator op)
            => op switch
            {
                FilterOperator.Equals =>
                    Equals(value, filterValue),

                FilterOperator.NotEquals =>
                    !Equals(value, filterValue),

                FilterOperator.GreaterThan =>
                    Compare(value, filterValue) > 0,

                FilterOperator.GreaterThanOrEqual =>
                    Compare(value, filterValue) >= 0,

                FilterOperator.LessThan =>
                    Compare(value, filterValue) < 0,

                FilterOperator.LessThanOrEqual =>
                    Compare(value, filterValue) <= 0,

                FilterOperator.Contains =>
                    StringMatch(value, filterValue, (a, b) =>
                        a.Contains(b, StringComparison.OrdinalIgnoreCase)),

                FilterOperator.StartsWith =>
                    StringMatch(value, filterValue, (a, b) =>
                        a.StartsWith(b, StringComparison.OrdinalIgnoreCase)),

                FilterOperator.EndsWith =>
                    StringMatch(value, filterValue, (a, b) =>
                        a.EndsWith(b, StringComparison.OrdinalIgnoreCase)),

                FilterOperator.In =>
                    filterValue is IEnumerable<object?> values &&
                    values.Any(v => Equals(value, v)),

                _ => true
            };

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
