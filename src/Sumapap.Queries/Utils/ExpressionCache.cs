using System.Linq.Expressions;

namespace Sumapap.Queries.Utils
{
    public static class ExpressionCache<T>
    {
        public static readonly ParameterExpression Param = Expression.Parameter(typeof(T), "p");
    }
}
