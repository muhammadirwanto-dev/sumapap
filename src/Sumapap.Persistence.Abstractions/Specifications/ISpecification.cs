using Sumapap.Queries.Abstractions;
using System.Linq.Expressions;

namespace Sumapap.Persistence.Abstractions.Specifications
{
    /// <summary>
    /// Defines a contract for the Specification pattern.
    /// Encapsulates query logic including filtering, ordering, and eager loading.
    /// </summary>
    /// <typeparam name="T">The type of the entity the specification is for.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// The core filter logic (The 'What').
        /// </summary>
        Expression<Func<T, bool>>? Criteria { get; }

        /// <summary>
        /// Gets the collection of related entity names to include in query results.
        /// 
        /// You can make it as multi-level as you want, e.g. "Orders.OrderItems.Product" to include related entities at multiple levels of the object graph.
        /// </summary>
        /// <remarks>Use this property to specify navigation properties that should be eagerly loaded when
        /// retrieving data. The strings in the collection represent the names of related entities to include.</remarks>
        IList<string> Includes { get; }

        /// <summary>
        /// The Query object containing Paging and Sorting parameters.
        /// </summary>
        IQuery? QueryOptions { get; }
    }
}
