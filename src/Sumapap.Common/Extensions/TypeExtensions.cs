namespace Sumapap.Common.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if the specified type is a nullable value type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a nullable value type; otherwise, false.</returns>
        public static bool IsNullableValueType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type GetClosedGeneric(this Type type)
        {
            return type
                .GetGenericTypeDefinition()
                .MakeGenericType(type.GenericTypeArguments);
        }
    }
}
