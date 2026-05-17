using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Represents a repository service registration configuration.
    /// Supports both generic (open) and non-generic (closed) repository types.
    /// </summary>
    public sealed record RepositoryRegistrationEntry(
        ServiceLifetime ServiceLifetime,
        Type AbstractType,
        Type ImplType,
        Type? EntityType,
        bool IsGeneric,
        bool AllowCaching,
        IRepositoryRegistrationEntryDecorator? Decorator);
}
