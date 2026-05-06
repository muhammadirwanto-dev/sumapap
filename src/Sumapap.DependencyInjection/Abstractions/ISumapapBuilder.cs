using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.DependencyInjection.Abstractions
{
    /// <summary>
    /// Provides a fluent builder for configuring Sumapap services through extension methods.
    /// </summary>
    /// <remarks>
    /// This class is intentionally simple - it only wraps <see cref="IServiceCollection"/>.
    /// All Sumapap-specific registration methods (repositories, handlers, etc.) are provided
    /// by extension methods in their respective libraries (e.g., Sumapap.Persistence, Sumapap.Queries).
    /// This design prevents circular dependencies and keeps the DI layer lightweight.
    /// </remarks>
    /// <param name="services">The service collection to configure.</param>
    public interface ISumapapBuilder
    {
        /// <summary>
        /// Gets the underlying service collection for registering services.
        /// </summary>
        /// <remarks>
        /// Extension methods in Sumapap libraries use this property to register services.
        /// Applications can also access this directly for custom service registrations
        /// that fall outside the fluent builder pattern.
        /// </remarks>
        public IServiceCollection Services { get; }
    }
}
