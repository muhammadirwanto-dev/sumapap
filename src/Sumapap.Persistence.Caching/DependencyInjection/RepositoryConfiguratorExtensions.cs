using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    public static class RepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Enables caching for this repository with detailed configuration.
        /// </summary>
        /// <param name="configurator">The repository configurator.</param>
        /// <param name="configure">Action to configure cache behavior, including which methods to cache.</param>
        /// <returns>The same configurator for method chaining.</returns>
        public static RepositoryConfigurator AllowCaching(this RepositoryConfigurator configurator, Action<RepositoryCacheConfiguration> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var registrationIndex = configurator.RegistrationIndex;
            var currentReg = configurator.Builder.Registrations[registrationIndex];
            var cacheConfig = new RepositoryCacheConfiguration();

            configure(cacheConfig);
            configurator.Builder.Registrations[registrationIndex] = currentReg with
            {
                AllowCaching = true,
                Decorator = new CachedRepositoryRegistration(currentReg, cacheConfig)
            };

            return configurator;
        }

        /// <summary>
        /// Enables caching for this repository with default configuration.
        /// All methods in CachedFunctionsMapping.Default will be cached.
        /// </summary>
        /// <param name="configurator">The repository configurator.</param>
        /// <returns>The same configurator for method chaining.</returns>
        public static RepositoryConfigurator AllowCaching(this RepositoryConfigurator configurator)
        {
            return configurator.AllowCaching(config =>
            {
                foreach (var kvp in CachedFunctionsMapping.Default)
                {
                    config.Methods[kvp.Key] = kvp.Value;
                }
            });
        }
    }
}
