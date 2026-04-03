using System.Globalization;

namespace Sumapap.Queries.Execution.Utils
{
    public static class ObjectComparer
    {
        public static int Compare(object? left, object? right)
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

        public static bool StringMatch(
            object? value,
            object? filterValue,
            Func<string, string, bool> matcher)
            => value != null && filterValue != null && matcher(value.ToString()!, filterValue.ToString()!);
    }
}
