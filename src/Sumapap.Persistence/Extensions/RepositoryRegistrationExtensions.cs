using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Extensions
{
    internal static class RepositoryRegistrationExtensions
    {
        extension(RepositoryRegistration registration)
        {
            public bool IsReadWriteRepository() => registration.ImplType.IsReadWriteRepository();

            public Type[] GetRepositoryInterfacesTypes() => registration.ImplType.GetRepositoryInterfacesTypes();
        }
    }
}
