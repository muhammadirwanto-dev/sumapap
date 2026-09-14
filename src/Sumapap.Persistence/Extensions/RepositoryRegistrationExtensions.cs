using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Extensions
{
    internal static class RepositoryRegistrationExtensions
    {
        public static bool IsReadWriteRepository(this RepositoryRegistration registration) => registration.ImplType.IsReadWriteRepository();

        public static Type[] GetRepositoryInterfacesTypes(this RepositoryRegistration registration) => registration.ImplType.GetRepositoryInterfacesTypes();
    }
}
