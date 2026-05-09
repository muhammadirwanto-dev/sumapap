using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.Persistence.DependencyInjection.Builder
{
    internal sealed record RepositoryServiceRegistration(
        ServiceLifetime ServiceLifetime,
        Type AbstractType,
        Type ImplType,
        Type EntityType,
        bool AllowCaching);
}
