using System.Linq.Expressions;
using Sumapap.Queries.Execution.EfCore.Expressions;
using Sumapap.Queries.Execution.Extensions;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.EfCore.Queryable
{
    internal static class EfQueryableFiltering
    {
        public static IQueryable<T> Apply<T>(IQueryable<T> source, FilterOptions options)
        {
            if (options?.RootGroup == null)
            {
                return source;
            }

            // One parameter to rule them all: 'x'
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = BuildGroupExpression<T>(options.RootGroup, parameter);

            if (body == null)
            {
                return source;
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(body, parameter));
        }

        private static Expression? BuildGroupExpression<T>(FilterGroup group, ParameterExpression parameter)
        {
            Expression? combined = null;

            foreach (var filter in group.Filters)
            {
                var leaf = BuildLeafExpression<T>(filter, parameter);
                combined = combined.Combine(leaf, group.Operator);
            }

            foreach (var subGroup in group.SubGroups)
            {
                var subs = BuildGroupExpression<T>(subGroup, parameter);
                combined = combined.Combine(subs, group.Operator);
            }

            return combined;
        }

        private static Expression BuildLeafExpression<T>(FilterDescriptor filter, ParameterExpression parameter)
        {
            return filter.Operator switch
            {
                FilterOperator.Equals => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.Equal, parameter),
                FilterOperator.NotEquals => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.NotEqual, parameter),
                FilterOperator.GreaterThan => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.GreaterThan, parameter),
                FilterOperator.GreaterThanOrEqual => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.GreaterThanOrEqual, parameter),
                FilterOperator.LessThan => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.LessThan, parameter),
                FilterOperator.LessThanOrEqual => ComparisonExpression.Build<T>(filter.Field, filter.Value, ExpressionType.LessThanOrEqual, parameter),
                FilterOperator.Contains => StringMethodExpression.Build<T>(filter.Field, filter.Value, nameof(string.Contains), parameter),
                FilterOperator.StartsWith => StringMethodExpression.Build<T>(filter.Field, filter.Value, nameof(string.StartsWith), parameter),
                FilterOperator.EndsWith => StringMethodExpression.Build<T>(filter.Field, filter.Value, nameof(string.EndsWith), parameter),
                FilterOperator.In => InExpression.Build<T>(filter.Field, filter.Value, parameter),
                _ => Expression.Constant(true)
            };
        }
    }
}
