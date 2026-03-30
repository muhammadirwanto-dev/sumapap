using System.Linq.Expressions;
using Sumapap.Queries.Execution.Extensions;
using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Queryable
{
    internal static class QueryableFiltering
    {
        public static IQueryable<T> Apply<T>(IQueryable<T> source, FilterOptions options)
        {
            if (options?.RootGroup == null)
            {
                return source;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var body = BuildGroupExpression<T>(options.RootGroup, parameter);

            if (body == null)
            {
                return source;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return source.Where(lambda);
        }

        private static Expression? BuildGroupExpression<T>(FilterGroup group, ParameterExpression parameter)
        {
            Expression? rootExpression = null;

            foreach (var filter in group.Filters)
            {
                var leafExpr = ExpressionCache.GetFilterExpression<T>(filter, parameter);
                rootExpression = rootExpression.Combine(leafExpr, group.Operator);
            }

            foreach (var subGroup in group.SubGroups)
            {
                var subExpr = BuildGroupExpression<T>(subGroup, parameter);
                rootExpression = rootExpression.Combine(subExpr, group.Operator);
            }

            return rootExpression;
        }
    }
}
