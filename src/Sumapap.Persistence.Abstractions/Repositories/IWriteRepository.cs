namespace Sumapap.Persistence.Abstractions.Repositories
{
    /// <summary>
    /// Defines a write-only repository for creating, updating, and deleting entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public interface IWriteRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds multiple entities.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <remarks>
        /// Implementation details vary (e.g., EF Core tracks changes, Dapper requires explicit UPDATE SQL).
        /// </remarks>
        void Update(TEntity entity);

        /// <summary>
        /// Updates multiple entities.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes multiple entities.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void DeleteRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to delete.</param>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        void Delete<TKey>(TKey id)
            where TKey : IEquatable<TKey>;

        /// <summary>
        /// Deletes multiple entities by their primary keys.
        /// </summary>
        /// <param name="ids">The collection of primary keys.</param>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        void DeleteRange<TKey>(IEnumerable<TKey> ids)
            where TKey : IEquatable<TKey>;

        /// <summary>
        /// Saves all changes to the underlying data store.
        /// </summary>
        void Save();

        /// <summary>
        /// Adds a new entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple entities asynchronously.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by its primary key asynchronously.
        /// </summary>
        /// <param name="id">The primary key of the entity to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>;

        /// <summary>
        /// Deletes multiple entities by their primary keys asynchronously.
        /// </summary>
        /// <param name="ids">The collection of primary keys.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>;

        /// <summary>
        /// Saves all changes to the underlying data store asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Defines a write-only repository with an associated context.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="TContext">The type of the data context.</typeparam>
    public interface IWriteRepository<TEntity, TContext> : IWriteRepository<TEntity>
        where TEntity : class;
}
