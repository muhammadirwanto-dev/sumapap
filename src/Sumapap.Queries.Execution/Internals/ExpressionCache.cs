using System.Collections.Concurrent;
using System.Linq.Expressions;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Execution.Internals
{
    public static class ExpressionCache
    {
        private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new();

        public static Expression<Func<T, bool>>? GetFilterExpression<T>(
            FilterDescriptor filter)
        {
            // simplified: build typed expression like discussed earlier
            // cache by (Type + Field + Operator)
            return DynamicExpressionBuilder.BuildPredicate<T>(filter);
        }

        public static IOrderedQueryable<T> ApplyOrdering<T>(
            IQueryable<T> source,
            SortDescriptor sort)
        {
            return DynamicExpressionBuilder.ApplyOrder(source, sort);
        }
    }
}
