using Microsoft.EntityFrameworkCore;
using Sumapap.Persistence.Abstractions.Entities;
using Sumapap.Persistence.Abstractions.Repositories;

namespace Sumapap.Persistence.EfCore.Repositories
{
    /// <summary>
    /// Provides a generic repository for performing create, update, and delete operations on entities using an Entity
    /// Framework Core DbContext.
    /// </summary>
    /// <remarks>This repository abstracts common write operations for entities, such as adding, updating, and
    /// deleting, and supports both synchronous and asynchronous patterns. Changes are not persisted to the database
    /// until Save or SaveAsync is called. This class is intended to be used as part of a unit of work pattern or in
    /// scenarios where explicit control over data persistence is required.</remarks>
    /// <typeparam name="TEntity">The type of entity managed by the repository. Must implement the IEntity interface.</typeparam>
    /// <typeparam name="TContext">The type of DbContext used for data access operations. Must inherit from DbContext.</typeparam>
    /// <param name="context">The DbContext instance used to access and manage entities.</param>
    public class WriteRepository<TEntity, TContext>(TContext @context) : IWriteRepository<TEntity, TContext>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        protected readonly TContext _context = @context;
        protected readonly DbSet<TEntity> _set = @context.Set<TEntity>();

        public void Add(TEntity entity)
        {
            _set.Add(entity);
        }

        public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _set.AddAsync(entity, cancellationToken);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _set.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return _set.AddRangeAsync(entities, cancellationToken);
        }

        public void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        public void Delete<TKey>(TKey id)
            where TKey : IEquatable<TKey>
        {
            var entity = _set.Find(id);
            if (entity != null)
            {
                _set.Remove(entity);
            }
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _set.Remove(entity);

            return Task.CompletedTask;
        }

        public async Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>
        {
            var entity = await _set.FindAsync(id, cancellationToken);
            if (entity != null)
            {
                _set.Remove(entity);
            }
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _set.RemoveRange(entities);
        }

        public void DeleteRange<TKey>(IEnumerable<TKey> ids)
            where TKey : IEquatable<TKey>
        {
            var entities = _set.AsNoTracking().Where(e => ids.Contains(((IEntity<TKey>)e).Id));
            if (entities.Any())
            {
                _set.RemoveRange(entities);
            }
        }

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            _set.RemoveRange(entities);

            return Task.CompletedTask;
        }

        public async Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>
        {
            var entities = _set.AsNoTracking().Where(e => ids.Contains(((IEntity<TKey>)e).Id));
            if (entities.Any())
            {
                _set.RemoveRange(entities);
            }
        }

        public void Update(TEntity entity)
        {
            _set.Update(entity);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _set.Update(entity);

            return Task.CompletedTask;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _set.UpdateRange(entities);
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            this.UpdateRange(entities);

            return Task.CompletedTask;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
