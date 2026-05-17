using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    internal sealed record CachedRepositoryRegistrationEntry(
        RepositoryRegistrationEntry RegistrationEntry,
        RepositoryCacheConfiguration Configuration) : IRepositoryRegistrationEntryDecorator;
}
