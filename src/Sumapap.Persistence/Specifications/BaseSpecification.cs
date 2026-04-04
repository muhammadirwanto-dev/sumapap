using System.Linq.Expressions;
using Sumapap.Persistence.Abstraction;
using Sumapap.Queries.Abstractions;

namespace Sumapap.Persistence.Specifications
{
    /// <summary>
    /// Base class for creating specifications, implementing ISpecification<T>.
    /// Provides helper methods to build the specification criteria.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseSpecification{T}"/> class
    /// with initial filter criteria.
    /// </remarks>
    /// <param name="criteria">The initial filter expression.</param>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        /// <inheritdoc/>
        public Expression<Func<T, bool>>? Criteria { get; }

        public IList<string> Includes { get; private set; } = [];

        public IQuery? QueryOptions { get; private set; }

        protected BaseSpecification()
        {
        }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected BaseSpecification(IList<string> includes)
        {
            Includes = includes;
        }

        protected BaseSpecification(Expression<Func<T, bool>> criteria, IList<string> includes)
        {
            Criteria = criteria;
            Includes = includes;
        }

        /// <summary>
        /// Adds an include expression for eager loading related data.
        /// </summary>
        /// <param name="includeExpression">The expression specifying the related data to include.</param>
        protected virtual void AddInclude(string include)
        {
            Includes.Add(include);
        }

        protected virtual void SetQuery(IQuery query)
        {
            QueryOptions = query;
        }
    }
}
