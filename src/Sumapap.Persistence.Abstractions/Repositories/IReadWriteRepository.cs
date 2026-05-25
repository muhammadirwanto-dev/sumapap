using Sumapap.Persistence.Abstractions.Entities;

namespace Sumapap.Persistence.Abstractions.Repositories
{
    public interface IReadWriteRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
        where TEntity : class, IEntity;

    public interface IReadWriteRepository<TEntity, TContext> : IReadWriteRepository<TEntity>,
        IReadRepository<TEntity, TContext>, IWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity;
}
