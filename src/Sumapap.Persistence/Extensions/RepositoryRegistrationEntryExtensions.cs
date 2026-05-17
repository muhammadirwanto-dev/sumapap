using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Extensions
{
    internal static class RepositoryRegistrationEntryExtensions
    {
        extension(RepositoryRegistrationEntry registration)
        {
            public bool IsReadWriteRepository()
                => typeof(IReadWriteRepository<,>).IsAssignableFrom(registration.ImplType);
        }
    }
}
