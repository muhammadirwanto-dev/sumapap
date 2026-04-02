using System.Reflection;
using Sumapap.Queries.Execution.Common;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Evaluators
{
    public static class SortEvaluator
    {
        public static bool EvaluateDescriptor<T>(SortDescriptor descriptor, IEnumerable<T> source, ref IOrderedEnumerable<T>? ordered)
        {
            var prop = ReflectionCache.GetProperty<T>(descriptor.Field);
            if (prop == null)
            {
                return false;
            }

            return EvaluateDescriptor(descriptor, source, ref ordered, prop);
        }

        public static bool EvaluateDescriptor<T>(SortDescriptor descriptor, IEnumerable<T> source, ref IOrderedEnumerable<T>? ordered, PropertyInfo prop)
        {
            ordered = ordered == null
                ? descriptor.Direction == SortDirection.Asc
                    ? source.OrderBy(x => prop.GetValue(x))
                    : source.OrderByDescending(x => prop.GetValue(x))
                : descriptor.Direction == SortDirection.Asc
                    ? ordered.ThenBy(x => prop.GetValue(x))
                    : ordered.ThenByDescending(x => prop.GetValue(x));

            return true;
        }

        public static bool EvaluateDescriptor<T>(SortDescriptor descriptor, IQueryable<T> source, ref IOrderedQueryable<T>? ordered)
        {
            var prop = ReflectionCache.GetProperty<T>(descriptor.Field);
            if (prop == null)
            {
                return false;
            }

            return EvaluateDescriptor(descriptor, source, ref ordered, prop);
        }

        public static bool EvaluateDescriptor<T>(SortDescriptor descriptor, IQueryable<T> source, ref IOrderedQueryable<T>? ordered, PropertyInfo prop)
        {
            ordered = ordered == null
                ? descriptor.Direction == SortDirection.Asc
                    ? source.OrderBy(x => prop.GetValue(x))
                    : source.OrderByDescending(x => prop.GetValue(x))
                : descriptor.Direction == SortDirection.Asc
                    ? ordered.ThenBy(x => prop.GetValue(x))
                    : ordered.ThenByDescending(x => prop.GetValue(x));

            return true;
        }
    }
}
