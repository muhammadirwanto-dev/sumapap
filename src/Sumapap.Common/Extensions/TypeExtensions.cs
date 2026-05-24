namespace Sumapap.Common.Extensions
{
    public static class TypeExtensions
    {
        extension(Type type)
        {
            /// <summary>
            /// Determines if the specified type is a nullable value type.
            /// </summary>
            /// <param name="type">The type to check.</param>
            /// <returns>True if the type is a nullable value type; otherwise, false.</returns>
            public bool IsNullableValueType()
            {
                return Nullable.GetUnderlyingType(type) != null;
            }

            public Type GetClosedGeneric()
            {
                return type
                    .GetGenericTypeDefinition()
                    .MakeGenericType(type.GenericTypeArguments);
            }
        }
    }
}
