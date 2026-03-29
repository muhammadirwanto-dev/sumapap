using System.Linq.Expressions;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Internals
{
    public static class DynamicExpressionBuilder
    {
        public static Expression<Func<T, bool>>? BuildPredicate<T>(
            FilterDescriptor filter)
        {
            var prop = ReflectionCache.GetProperty<T>(filter.Field);
            if (prop == null)
                return null;

            var param = Expression.Parameter(typeof(T), "e");
            var left = Expression.Property(param, prop);
            var right = Expression.Constant(
                Convert.ChangeType(filter.Value, prop.PropertyType),
                prop.PropertyType);

            Expression body = filter.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(left, right),
                FilterOperator.NotEquals => Expression.NotEqual(left, right),
                FilterOperator.GreaterThan => Expression.GreaterThan(left, right),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
                FilterOperator.LessThan => Expression.LessThan(left, right),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(left, right),

                FilterOperator.Contains =>
                    Expression.Call(left,
                        nameof(string.Contains),
                        null,
                        right),

                FilterOperator.StartsWith =>
                    Expression.Call(left,
                        nameof(string.StartsWith),
                        null,
                        right),

                FilterOperator.EndsWith =>
                    Expression.Call(left,
                        nameof(string.EndsWith),
                        null,
                        right),

                _ => null!
            };

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source,
            SortDescriptor sort)
        {
            var prop = ReflectionCache.GetProperty<T>(sort.Field)
                ?? throw new InvalidOperationException(
                    $"Sort field '{sort.Field}' not found.");

            var param = Expression.Parameter(typeof(T), "e");
            var body = Expression.Property(param, prop);
            var lambda = Expression.Lambda(body, param);

            var methodName = sort.Direction == SortDirection.Asc
                ? nameof(System.Linq.Queryable.OrderBy)
                : nameof(System.Linq.Queryable.OrderByDescending);

            var method = typeof(System.Linq.Queryable)
                .GetMethods()
                .Single(m =>
                    m.Name == methodName &&
                    m.GetParameters().Length == 2);

            return (IOrderedQueryable<T>)method
                .MakeGenericMethod(typeof(T), prop.PropertyType)
                .Invoke(null, new object[] { source, lambda })!;
        }
    }

}