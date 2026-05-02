using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sumapap.Queries.Execution.Utils;

namespace Sumapap.Queries.Execution.EfCore.Expressions
{
    internal class ComparisonExpression
    {
        public static Expression<Func<T, bool>> Build<T>(
            string field,
            object? value,
            ExpressionType comparisonType,
            ParameterExpression? parameter = null)
        {
            var param = parameter ?? ExpressionCache<T>.Param;
            var property = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(object)],
                param,
                Expression.Constant(field));

            var constant = Expression.Constant(value);
            var converted = Expression.Convert(constant, property.Type);

            var body = Expression.MakeBinary(
                comparisonType,
                Expression.Convert(property, property.Type),
                converted);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
