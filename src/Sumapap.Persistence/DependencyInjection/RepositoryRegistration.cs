using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.Persistence.DependencyInjection
{
    internal sealed record RepositoryRegistration(
        ServiceLifetime ServiceLifetime,
        Type AbstractType,
        Type ImplType,
        Type EntityType,
        bool AllowCaching);
}
