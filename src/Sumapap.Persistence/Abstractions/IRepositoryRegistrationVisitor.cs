using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Abstractions;

/// <summary>
/// Visitor interface for processing repository registrations.
/// Implementations can apply cross-cutting concerns like caching, logging, or validation.
/// </summary>
public interface IRepositoryRegistrationVisitor
{
    /// <summary>
    /// Visit a repository registration entry and optionally modify the service collection.
    /// </summary>
    /// <param name="entry">The repository registration entry to visit.</param>
    /// <param name="services">The service collection to potentially modify.</param>
    void Visit(RepositoryRegistrationEntry entry, IServiceCollection services);
}
