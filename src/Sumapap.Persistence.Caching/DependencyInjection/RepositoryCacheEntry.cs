namespace Sumapap.Persistence.Caching.DependencyInjection
{
    /// <summary>
    /// Represents a single repository registration with cache configuration.
    /// </summary>
    public sealed class RepositoryCacheEntry
    {
        /// <summary>
        /// The repository implementation type.
        /// </summary>
        public required Type RepositoryType { get; init; }

        /// <summary>
        /// Cache configuration for this repository.
        /// </summary>
        public required RepositoryCacheConfiguration Configuration { get; init; }

        /// <summary>
        /// The abstract service type(s) registered for this repository.
        /// E.g., IRepository&lt;User&gt;, IReadRepository&lt;User&gt;, etc.
        /// </summary>
        public List<Type> ServiceTypes { get; init; } = [];

        public bool IsCached(string key) => Configuration.IsCached(key);
    }
}
