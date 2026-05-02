using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Queries.Execution.Utils;

namespace Sumapap.Queries.Execution.EfCore.Expressions
{
    internal class StringMethodExpression
    {
        public static Expression<Func<T, bool>> Build<T>(
            string field,
            object? value,
            string methodName,
            ParameterExpression? parameter = null)
        {
            var param = parameter ?? ExpressionCache<T>.Param;
            var property = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(string)],
                param,
                Expression.Constant(field));

            var constant = Expression.Constant(value?.ToString());
            var method = typeof(string).GetMethod(
                methodName,
                [typeof(string)])!;

            var body = Expression.Call(property, method, constant);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
