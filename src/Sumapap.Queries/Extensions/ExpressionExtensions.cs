using Sumapap.Queries.Abstractions.Filtering;
using System.Linq.Expressions;

namespace Sumapap.Queries.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression? Combine(this Expression? expression, Expression? other, CompositeOperator op)
        {
            if (expression == null)
                return other;
            if (other == null)
                return expression;

            return op == CompositeOperator.And
                ? Expression.AndAlso(expression, other)
                : Expression.OrElse(expression, other);
        }
    }
}
