using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Sumapap.Queries.Execution.EfCore.Expressions
{
    internal class StringMethodExpression
    {
        public static Expression<Func<T, bool>> Build<T>(
            string field,
            object? value,
            string methodName)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(string)],
                parameter,
                Expression.Constant(field));

            var constant = Expression.Constant(value?.ToString());
            var method = typeof(string).GetMethod(
                methodName,
                [typeof(string)])!;

            var body = Expression.Call(property, method, constant);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
