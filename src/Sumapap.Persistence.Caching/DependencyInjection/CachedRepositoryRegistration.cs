using Sumapap.Persistence.DependencyInjection;
using Sumapap.Persistence.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    internal sealed record CachedRepositoryRegistration(
        RepositoryRegistration Registration,
        RepositoryCacheConfiguration Configuration) : IRepositoryRegistrationDecorator;
}
