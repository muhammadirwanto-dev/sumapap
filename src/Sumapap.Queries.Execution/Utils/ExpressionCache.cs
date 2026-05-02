using System.Linq.Expressions;

namespace Sumapap.Queries.Execution.Utils
{
    public static class ExpressionCache<T>
    {
        public static readonly ParameterExpression Param = Expression.Parameter(typeof(T), "p");
    }
}
