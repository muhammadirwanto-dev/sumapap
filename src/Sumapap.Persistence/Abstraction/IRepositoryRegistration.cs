using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.Abstraction
{
    /// <summary>
    /// Fluent configuration for a specific repository registration.
    /// </summary>
    /// <typeparam name="TImpl">The repository implementation type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IRepositoryRegistration<TImpl, TEntity>
        where TImpl : class
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Gets the parent builder to continue configuration.
        /// </summary>
        ISumapapBuilder Builder { get; }

        /// <summary>
        /// Enables caching for this specific repository.
        /// </summary>
        /// <returns>This registration for further configuration.</returns>
        IRepositoryRegistration<TImpl, TEntity> UseCache();

        /// <summary>
        /// Configures repository-specific options.
        /// </summary>
        /// <param name="configure">Action to configure repository options.</param>
        /// <returns>This registration for further configuration.</returns>
        IRepositoryRegistration<TImpl, TEntity> WithOptions(Action<RepositoryOptions> configure);
    }

    /// <summary>
    /// Options for configuring a repository registration.
    /// </summary>
    public class RepositoryOptions
    {
        /// <summary>
        /// Gets or sets whether caching is enabled for this repository.
        /// </summary>
        public bool EnableCache { get; set; }

        /// <summary>
        /// Gets or sets the cache expiration time in seconds.
        /// </summary>
        public int? CacheExpirationSeconds { get; set; }

        /// <summary>
        /// Gets or sets custom configuration metadata.
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
