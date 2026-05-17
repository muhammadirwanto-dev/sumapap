using System.Collections;
using System.Linq.Expressions;
using Sumapap.Queries.Filtering;

namespace Sumapap.Queries.Execution.Utils
{
    public sealed class ExpressionBuilder<T>
    {
        public Expression<Func<T, bool>> BuildFilterExpression(FilterGroup group)
        {
            var parameter = ExpressionCache<T>.Param;
            var body = BuildGroupExpression(group, parameter);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public Expression<Func<T, object>> BuildSortExpression(string propertyName)
        {
            var parameter = ExpressionCache<T>.Param;
            var property = Expression.Property(parameter, propertyName);
            var converted = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(converted, parameter);
        }

        private static Expression BuildGroupExpression(FilterGroup group, ParameterExpression param)
        {
            Expression? root = null;
            bool isAnd = group.Operator == CompositeOperator.And;

            foreach (var filter in group.Filters)
            {
                var filterExpr = BuildDescriptorExpression(filter, param);
                root = Combine(root, filterExpr, group.Operator);
            }

            foreach (var subGroup in group.SubGroups)
            {
                var subExpr = BuildGroupExpression(subGroup, param);
                root = Combine(root, subExpr, group.Operator);
            }

            return root ?? Expression.Constant(isAnd);
        }

        private static Expression BuildDescriptorExpression(FilterDescriptor descriptor, ParameterExpression param)
        {
            var prop = Expression.Property(param, descriptor.Field);
            var constant = Expression.Constant(descriptor.Value);

            return descriptor.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(prop, constant),
                FilterOperator.NotEquals => Expression.NotEqual(prop, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(prop, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(prop, constant),
                FilterOperator.LessThan => Expression.LessThan(prop, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(prop, constant),
                FilterOperator.Contains => Expression.Call(prop, typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!, constant),
                FilterOperator.StartsWith => Expression.Call(prop, typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!, constant),
                FilterOperator.EndsWith => Expression.Call(prop, typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!, constant),
                FilterOperator.In => BuildInExpression(prop, descriptor.Value),
                _ => Expression.Constant(true)
            };
        }

        private static Expression BuildInExpression(MemberExpression property, object? values)
        {
            if (values is not IEnumerable valueList)
            {
                return Expression.Constant(false);
            }

            var propertyType = property.Type;
            var method = typeof(Enumerable)
                .GetMethods()
                .Where(m => m.Name == nameof(Enumerable.Contains))
                .Single(m => m.GetParameters().Length == 2)
                .MakeGenericMethod(propertyType);

            // To ensure compatibility with EF Core and Enumerable, 
            // it's best to cast the input values to the correct List<T>
            var castedList = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.Cast))!
                .MakeGenericMethod(propertyType)
                .Invoke(null, [values]);

            var listConstant = Expression.Constant(castedList);

            return Expression.Call(null, method, listConstant, property);
        }

        private static Expression Combine(Expression? current, Expression next, CompositeOperator op)
        {
            return current == null
                ? next
                : op == CompositeOperator.And
                    ? Expression.AndAlso(current, next)
                    : Expression.OrElse(current, next);
        }
    }
}
