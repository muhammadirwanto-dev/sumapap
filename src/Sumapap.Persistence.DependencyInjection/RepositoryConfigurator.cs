using Sumapap.Persistence.Abstraction;
using Sumapap.Persistence.DependencyInjection.Builder;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Fluent configurator for individual repository registration, enabling opt-in caching.
    /// </summary>
    public sealed class RepositoryConfigurator<TRepository, TEntity> : RepositoryConfigurator
        where TRepository : class
        where TEntity : class, IEntity
    {
        internal RepositoryConfigurator(RepositoryRegistrationBuilder builder, int registrationIndex)
            : base(builder, registrationIndex)
        {
        }
    }

    /// <summary>
    /// Fluent configurator for repository registration, enabling opt-in caching.
    /// Supports both generic (open) and non-generic (closed) repository types.
    /// </summary>
    public class RepositoryConfigurator
    {
        private readonly RepositoryRegistrationBuilder _builder;
        private readonly int _registrationIndex;

        internal RepositoryConfigurator(RepositoryRegistrationBuilder builder, int registrationIndex)
        {
            _builder = builder;
            _registrationIndex = registrationIndex;
        }

        /// <summary>
        /// Enables caching for this repository with detailed configuration.
        /// </summary>
        /// <param name="configure">Action to configure cache behavior, including which methods to cache.</param>
        /// <returns>The same configurator for method chaining.</returns>
        public RepositoryConfigurator AllowCaching(
            Action<RepositoryCacheConfiguration> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var currentReg = _builder._registrations[_registrationIndex];
            var cacheConfig = new RepositoryCacheConfiguration();

            configure(cacheConfig);

            _builder._registrations[_registrationIndex] = currentReg with
            {
                AllowCaching = true,
                CachingConfiguration = cacheConfig
            };

            return this;
        }

        /// <summary>
        /// Enables caching for this repository with default configuration.
        /// All methods in CachedFunctionsMapping.Default will be cached.
        /// </summary>
        /// <returns>The same configurator for method chaining.</returns>
        public RepositoryConfigurator AllowCaching()
        {
            return AllowCaching(config =>
            {
                // Use default method mappings
                foreach (var kvp in CachedFunctionsMapping.Default)
                {
                    config.Methods[kvp.Key] = kvp.Value;
                }
            });
        }

        /// <summary>
        /// Returns to the builder to add more repositories.
        /// </summary>
        public RepositoryRegistrationBuilder Builder => _builder;
    }
}
