using System.Linq.Expressions;
using Sumapap.Queries.Abstractions.Filtering;

namespace Sumapap.Queries.Extensions
{
    public static class ExpressionExtensions
    {
        extension(Expression? expression)
        {
            public Expression? Combine(Expression? other, CompositeOperator op)
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
}
