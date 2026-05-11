using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.Caching
{
    public sealed class CachedFunctionsMapping : Dictionary<string, bool>
    {
        public static readonly CachedFunctionsMapping Default = new()
        {
            { nameof(IReadRepository<>.Find), true },
            { nameof(IReadRepository<>.FindAsync), true },
            { nameof(IReadRepository<>.FirstOrDefault), true },
            { nameof(IReadRepository<>.FirstOrDefaultAsync), true },
            { nameof(IReadRepository<>.GetAll), true },
            { nameof(IReadRepository<>.GetAllAsync), true },
            { nameof(IReadRepository<>.SingleOrDefault), true },
            { nameof(IReadRepository<>.SingleOrDefaultAsync), true },
            { nameof(IReadRepository<>.StreamAllAsync), true },
            { nameof(IReadRepository<>.StreamWhereAsync), true },
        };
    }
}
