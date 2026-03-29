using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryableFiltering
    {
        public static IQueryable<T> Apply<T>(
            IQueryable<T> source,
            FilterOptions filters)
        {
            foreach (var filter in filters.Filters)
            {
                var expr = ExpressionCache.GetFilterExpression<T>(filter);
                if (expr != null)
                {
                    source = source.Where(expr);
                }
            }

            return source;
        }
    }
}
