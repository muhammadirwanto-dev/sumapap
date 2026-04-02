using Microsoft.EntityFrameworkCore;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.EfCore.Queryable
{
    internal static class EfQueryableSorting
    {
        public static IQueryable<T> Apply<T>(
            IQueryable<T> source,
            SortOptions sort)
        {
            if (sort == null || sort.Sorts.Count == 0)
            {
                return source;
            }

            IOrderedQueryable<T>? ordered = null;

            foreach (var descriptor in sort.Sorts)
            {
                ordered = ordered == null
                    ? descriptor.Direction == SortDirection.Asc
                        ? source.OrderBy(e => EF.Property<object>(e!, descriptor.Field))
                        : source.OrderByDescending(e => EF.Property<object>(e!, descriptor.Field))
                    : descriptor.Direction == SortDirection.Asc
                        ? ordered.ThenBy(e => EF.Property<object>(e!, descriptor.Field))
                        : ordered.ThenByDescending(e => EF.Property<object>(e!, descriptor.Field));
            }

            return ordered ?? source;
        }
    }
}
