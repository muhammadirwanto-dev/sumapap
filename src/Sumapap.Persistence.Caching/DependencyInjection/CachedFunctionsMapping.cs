using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    public sealed class CachedFunctionsMapping : Dictionary<string, bool>
    {
        public static readonly CachedFunctionsMapping Default = new()
        {
            { nameof(IReadRepository<>.Count), true },
            { nameof(IReadRepository<>.CountAsync), true },
            { nameof(IReadRepository<>.DetatchFromTracking), false},
            { nameof(IReadRepository<>.Find), true },
            { nameof(IReadRepository<>.FindAsync), true },
            { nameof(IReadRepository<>.FirstOrDefault), true },
            { nameof(IReadRepository<>.FirstOrDefaultAsync), true },
            { nameof(IReadRepository<>.GetAll), true },
            { nameof(IReadRepository<>.GetAllAsync), true },
            { nameof(IReadRepository<>.IsExists), false},
            { nameof(IReadRepository<>.IsExistsAsync), false},
            { nameof(IReadRepository<>.QueryAsync), false},
            { nameof(IReadRepository<>.SingleOrDefault), true },
            { nameof(IReadRepository<>.SingleOrDefaultAsync), true },
            { nameof(IReadRepository<>.StreamAllAsync), false },
            { nameof(IReadRepository<>.StreamWhereAsync), false },
            { nameof(IReadRepository<>.Where), true},
            { nameof(IReadRepository<>.WhereAsync), true},
        };
    }
}
