using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Represents a repository service registration configuration.
    /// Supports both generic (open) and non-generic (closed) repository types.
    /// </summary>
    internal sealed record RepositoryRegistrationEntry(
        ServiceLifetime ServiceLifetime,
        Type AbstractType,
        Type ImplType,
        Type? EntityType,
        bool IsGeneric,
        bool AllowCaching,
        RepositoryCacheConfiguration? CachingConfiguration = null);
}
