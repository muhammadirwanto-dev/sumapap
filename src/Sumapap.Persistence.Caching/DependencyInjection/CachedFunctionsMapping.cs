#if !NET10_0_OR_GREATER
using Sumapap.Persistence.Abstractions.Entities;
#endif // !NET10_0_OR_GREATER
using Sumapap.Persistence.Abstractions.Repositories;

namespace Sumapap.Persistence.Caching.DependencyInjection
{
    public sealed class CachedFunctionsMapping : Dictionary<string, bool>
    {
        public static readonly CachedFunctionsMapping Default = new()
        {
#if NET10_0_OR_GREATER
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
#else
            { nameof(IReadRepository<IEntity>.Count), true },
            { nameof(IReadRepository<IEntity>.CountAsync), true },
            { nameof(IReadRepository<IEntity>.DetatchFromTracking), false},
            { nameof(IReadRepository<IEntity>.Find), true },
            { nameof(IReadRepository<IEntity>.FindAsync), true },
            { nameof(IReadRepository<IEntity>.FirstOrDefault), true },
            { nameof(IReadRepository<IEntity>.FirstOrDefaultAsync), true },
            { nameof(IReadRepository<IEntity>.GetAll), true },
            { nameof(IReadRepository<IEntity>.GetAllAsync), true },
            { nameof(IReadRepository<IEntity>.IsExists), false},
            { nameof(IReadRepository<IEntity>.IsExistsAsync), false},
            { nameof(IReadRepository<IEntity>.QueryAsync), false},
            { nameof(IReadRepository<IEntity>.SingleOrDefault), true },
            { nameof(IReadRepository<IEntity>.SingleOrDefaultAsync), true },
            { nameof(IReadRepository<IEntity>.StreamAllAsync), false },
            { nameof(IReadRepository<IEntity>.StreamWhereAsync), false },
            { nameof(IReadRepository<IEntity>.Where), true},
            { nameof(IReadRepository<IEntity>.WhereAsync), true},
#endif
        };
    }
}
