namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Configuration for repository caching behavior.
    /// </summary>
    public sealed class RepositoryCacheConfiguration
    {
        /// <summary>
        /// Mapping of method names to whether they should be cached.
        /// </summary>
        public CachedFunctionsMapping Methods { get; set; } = [];

        /// <summary>
        /// Cache entry duration. If null, uses default cache provider settings.
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Additional metadata for cache provider-specific configuration.
        /// </summary>
        public Dictionary<string, object> Metadata { get; } = [];
    }
}
