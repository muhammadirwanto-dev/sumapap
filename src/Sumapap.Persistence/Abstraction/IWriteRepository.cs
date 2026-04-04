namespace Sumapap.Persistence.Abstraction
{
    public interface IWriteRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        void Add(TEntity entity);

        /// <summary>
        /// Adds multiple entities.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <remarks>
        /// Implementation details vary (e.g., EF Core tracks changes, Dapper requires explicit UPDATE SQL).
        /// </remarks>
        /// <returns>Task representing the asynchronous operation.</returns>
        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        void Delete(TEntity entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key of the entity to delete.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        void Delete<TKey>(TKey id)
            where TKey : IEquatable<TKey>;

        void DeleteRange<TKey>(IEnumerable<TKey> ids)
            where TKey : IEquatable<TKey>;

        void Save();

        ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>;

        Task DeleteRangeAsync<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>;

        Task SaveAsync(CancellationToken cancellationToken = default);
    }

    public interface IWriteRepository<TEntity, TContext> : IWriteRepository<TEntity>
        where TEntity : class;
}
