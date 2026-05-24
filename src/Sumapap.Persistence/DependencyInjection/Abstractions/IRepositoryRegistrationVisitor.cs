using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.Persistence.DependencyInjection.Abstractions;

/// <summary>
/// Visitor interface for processing repository registrations.
/// Implementations can apply cross-cutting concerns like caching, logging, or validation.
/// </summary>
public interface IRepositoryRegistrationVisitor
{
    /// <summary>
    /// Visit a repository registration entry and optionally modify the service collection.
    /// </summary>
    /// <param name="registration">The repository registration entry to visit.</param>
    /// <param name="services">The service collection to potentially modify.</param>
    void Visit(RepositoryRegistration registration, IServiceCollection services);
}
