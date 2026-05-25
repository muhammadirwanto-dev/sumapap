namespace Sumapap.Persistence.Abstractions.Entities
{
    /// <summary>
    /// Defines a contract for an entity type that can be used as a base for domain or data model objects.
    /// </summary>
    /// <remarks>Implement this interface to indicate that a type represents an entity within a domain model
    /// or persistence layer. The specific requirements and members for entities are determined by the implementing type
    /// or framework.</remarks>
    public interface IEntity;

    /// <summary>
    /// Defines an entity with a strongly typed identifier.
    /// </summary>
    /// <remarks>This interface is typically used as a base for domain entities that require a unique
    /// identifier of a specific type. Implementations should ensure that the <c>Id</c> property uniquely identifies
    /// each entity instance.</remarks>
    /// <typeparam name="TKey">The type of the entity's identifier. Must implement <see cref="IEquatable{TKey}"/> to support value equality.</typeparam>
    public interface IEntity<TKey> : IEntity
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
