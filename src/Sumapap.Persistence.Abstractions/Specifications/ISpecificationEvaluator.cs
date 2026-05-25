namespace Sumapap.Persistence.Abstractions.Specifications
{
    /// <summary>
    /// Utility class to apply ISpecification logic to an IQueryable.
    /// This centralizes the logic for translating specification properties
    /// into Entity Framework Core LINQ operations.
    /// </summary>
    public interface ISpecificationEvaluator
    {
        /// <summary>
        /// Applies the given specification to the input queryable.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity being queried.</typeparam>
        /// <param name="inputQuery">The input IQueryable, typically a DbSet or an already partially filtered query.</param>
        /// <param name="specification">The specification object containing the query criteria (filter, includes, order, paging).</param>
        /// <param name="evaluateCriteriaOnly">
        /// If true, only applies the filtering Criteria (specification.Criteria).
        /// This is an optimization used for CountAsync and ExistsAsync operations where includes, ordering,
        /// and paging are unnecessary and would add overhead. Defaults to false.
        /// </param>
        /// <returns>An IQueryable<TEntity> with the specification's logic applied.</returns>
        public IQueryable<TEntity> Evaluate<TEntity>(IQueryable<TEntity> query, ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false)
            where TEntity : class;
    }
}
