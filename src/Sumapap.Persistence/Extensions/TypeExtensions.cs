using Sumapap.Persistence.Abstractions.Repositories;

namespace Sumapap.Persistence.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsReadWriteRepository(this Type type)
            => type.GetInterfaces()
                .Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IReadWriteRepository<>));

        public static Type[] GetRepositoryInterfacesTypes(this Type type)
            => [.. type.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    typeof(IRepository).IsAssignableFrom(i.GetGenericTypeDefinition()))];
    }
}
