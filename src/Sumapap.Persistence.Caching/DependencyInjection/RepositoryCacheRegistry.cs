using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    /// <summary>
    /// Registry tracking all repository registrations with their cache configurations.
    /// This will be consumed by cache providers (e.g., FusionCache) to apply decorators.
    /// </summary>
    public sealed class RepositoryCacheRegistry
    {
        private readonly List<RepositoryCacheEntry> _entries = [];

        /// <summary>
        /// All registered repositories with caching enabled.
        /// </summary>
        public IReadOnlyList<RepositoryCacheEntry> CachedRepositories => _entries.AsReadOnly();

        /// <summary>
        /// Registers a repository with caching configuration.
        /// </summary>
        internal void Register(RepositoryCacheEntry entry)
        {
            ArgumentNullException.ThrowIfNull(entry);
            _entries.Add(entry);
        }

        /// <summary>
        /// Clears all registrations (for testing purposes).
        /// </summary>
        internal void Clear() => _entries.Clear();

        internal RepositoryCacheEntry? GetCacheEntry<T>(T repository)
            where T : IRepository
        {
            return CachedRepositories
                .FirstOrDefault(x =>
                {
                    return x.RepositoryType == repository.GetType();
                });
        }
    }
}
