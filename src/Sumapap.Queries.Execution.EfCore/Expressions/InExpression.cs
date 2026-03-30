using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Sumapap.Queries.Execution.EfCore.Expressions
{
    internal class InExpression
    {
        public static Expression<Func<T, bool>> Build<T>(
            string field,
            object? value,
            ParameterExpression? parameter = null)
        {
            if (value is not IEnumerable values)
            {
                throw new InvalidOperationException("FilterOperator.In requires IEnumerable value.");
            }

            var param = parameter ?? Expression.Parameter(typeof(T), "p");
            var property = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(object)],
                param,
                Expression.Constant(field));

            var list = values.Cast<object?>().ToList();
            var constant = Expression.Constant(list);

            var containsMethod = typeof(List<object?>)
                .GetMethod(nameof(List<>.Contains))!;

            var body = Expression.Call(constant, containsMethod, property);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
