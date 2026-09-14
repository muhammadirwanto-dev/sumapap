namespace Sumapap.Persistence.Abstractions.Repositories
{
    /// <summary>
    /// Base marker interface for all repository types.
    /// </summary>
    public interface IRepository;

    /// <summary>
    /// Base marker interface for repositories that manage entities of a specific type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
    public interface IRepository<TEntity> : IRepository
    {
        /// <summary>
        /// Gets the underlying context associated with the repository.
        /// </summary>
        /// <typeparam name="TContext">The context type.</typeparam>
        /// <returns>The context instance.</returns>
        TContext GetContext<TContext>();

        /// <summary>
        /// Tries to get the underlying context associated with the repository.
        /// </summary>
        /// <typeparam name="TContext">The context type.</typeparam>
        /// <param name="context">The context instance if found; otherwise, null.</param>
        /// <returns>True if the context is associated with the repository; otherwise, false.</returns>
        bool TryGetContext<TContext>(out TContext? context);
    }
}
