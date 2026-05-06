using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.Abstraction;

namespace Sumapap.Persistence.DependencyInjection
{
    internal class RepositoryRegistration<TImpl, TEntity>(ISumapapBuilder builder)
        : IRepositoryRegistration<TImpl, TEntity>
        where TImpl : class
        where TEntity : class, IEntity
    {
        private readonly RepositoryOptions _options = new();

        public ISumapapBuilder Builder => builder;

        public IRepositoryRegistration<TImpl, TEntity> UseCache()
        {
            _options.EnableCache = true;

            // Mark this specific repository for caching
            // The actual decoration happens in PersistenceConfiguration
            var metadata = Builder.Services.GetOrCreatePersistenceMetadata();
            metadata.CachedRepositories.Add(typeof(TImpl));

            return this;
        }

        public IRepositoryRegistration<TImpl, TEntity> WithOptions(Action<RepositoryOptions> configure)
        {
            configure?.Invoke(_options);

            // Store options for this repository type
            var metadata = Builder.Services.GetOrCreatePersistenceMetadata();
            metadata.RepositoryOptions[typeof(TImpl)] = _options;

            return this;
        }
    }
}
