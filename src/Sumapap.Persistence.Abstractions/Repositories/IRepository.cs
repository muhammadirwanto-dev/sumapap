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
    public interface IRepository<TEntity> : IRepository;
}
