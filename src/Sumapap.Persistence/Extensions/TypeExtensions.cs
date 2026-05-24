using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.Extensions
{
    public static class TypeExtensions
    {
        extension(Type type)
        {
            public bool IsReadWriteRepository()
                => type.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IReadWriteRepository<>));

            public Type[] GetRepositoryInterfacesTypes()
                => [.. type.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        typeof(IRepository).IsAssignableFrom(i.GetGenericTypeDefinition()))];
        }
    }
}
