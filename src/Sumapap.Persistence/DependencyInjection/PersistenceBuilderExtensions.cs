using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.Abstraction;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="ISumapapBuilder"/> to register persistence services.
    /// </summary>
    public static class PersistenceBuilderExtensions
    {
        extension(ISumapapBuilder builder)
        {
            /// <summary>
            /// Registers a scoped repository implementation.
            /// </summary>
            /// <typeparam name="TImpl">The repository implementation type.</typeparam>
            /// <typeparam name="TEntity">The entity type.</typeparam>
            /// <param name="builder">The Sumapap builder.</param>
            /// <returns>A repository registration builder for further configuration.</returns>
            public IRepositoryRegistration<TImpl, TEntity> AddScopedRepository<TImpl, TEntity>()
                where TImpl : class
                where TEntity : class, IEntity
            {
                builder.Services.AddScopedRepository<TImpl, TEntity>();
                return new RepositoryRegistration<TImpl, TEntity>(builder);
            }

            /// <summary>
            /// Registers a scoped repository implementation with a service interface.
            /// </summary>
            /// <typeparam name="TService">The service interface type.</typeparam>
            /// <typeparam name="TImpl">The repository implementation type.</typeparam>
            /// <typeparam name="TEntity">The entity type.</typeparam>
            /// <param name="builder">The Sumapap builder.</param>
            /// <returns>A repository registration builder for further configuration.</returns>
            public IRepositoryRegistration<TImpl, TEntity> AddScopedRepository<TService, TImpl, TEntity>()
                where TService : class
                where TImpl : class, TService
                where TEntity : class, IEntity
            {
                builder.Services.AddScopedRepository<TService, TImpl, TEntity>();
                return new RepositoryRegistration<TImpl, TEntity>(builder);
            }

            /// <summary>
            /// Registers a transient repository implementation.
            /// </summary>
            /// <typeparam name="TImpl">The repository implementation type.</typeparam>
            /// <typeparam name="TEntity">The entity type.</typeparam>
            /// <param name="builder">The Sumapap builder.</param>
            /// <returns>A repository registration builder for further configuration.</returns>
            public IRepositoryRegistration<TImpl, TEntity> AddTransientRepository<TImpl, TEntity>()
                where TImpl : class
                where TEntity : class, IEntity
            {
                builder.Services.AddTransientRepository<TImpl, TEntity>();
                return new RepositoryRegistration<TImpl, TEntity>(builder);
            }

            /// <summary>
            /// Registers a transient repository implementation with a service interface.
            /// </summary>
            /// <typeparam name="TService">The service interface type.</typeparam>
            /// <typeparam name="TImpl">The repository implementation type.</typeparam>
            /// <typeparam name="TEntity">The entity type.</typeparam>
            /// <param name="builder">The Sumapap builder.</param>
            /// <returns>A repository registration builder for further configuration.</returns>
            public IRepositoryRegistration<TImpl, TEntity> AddTransientRepository<TService, TImpl, TEntity>()
                where TService : class
                where TImpl : class, TService
                where TEntity : class, IEntity
            {
                builder.Services.AddTransientRepository<TService, TImpl, TEntity>();
                return new RepositoryRegistration<TImpl, TEntity>(builder);
            }

            /// <summary>
            /// Enables caching for all registered repositories globally.
            /// All repositories will be decorated with a caching implementation.
            /// </summary>
            /// <param name="builder">The Sumapap builder.</param>
            /// <param name="configure">Optional action to configure global cache settings.</param>
            /// <returns>The Sumapap builder for method chaining.</returns>
            /// <remarks>
            /// This should be called after all repository registrations.
            /// The actual cache decorator (CachedRepository) will be implemented in Sumapap.Persistence.Caches.
            /// </remarks>
            public ISumapapBuilder UseCaches(Action<CacheOptions>? configure = null)
            {
                var metadata = builder.Services.GetOrCreatePersistenceMetadata();
                metadata.GlobalCachingEnabled = true;

                var options = new CacheOptions();
                configure?.Invoke(options);

                // Store global cache options
                builder.Services.AddSingleton(options);

                // TODO: When Sumapap.Persistence.Caches is implemented:
                // - Decorate all IRepository<T>, IReadRepository<T>, IReadWriteRepository<T> with CachedRepository<T>
                // - Apply cache options to decorators
                // Example (conceptual):
                // builder.Services.Decorate(typeof(IReadRepository<>), typeof(CachedReadRepository<>));

                return builder;
            }
        }
    }
}
