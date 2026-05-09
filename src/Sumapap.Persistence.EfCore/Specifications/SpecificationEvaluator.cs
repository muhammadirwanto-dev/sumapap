using Microsoft.EntityFrameworkCore;
using Sumapap.Persistence.Abstraction;
using Sumapap.Queries.Execution.EfCore.Extensions;

namespace Sumapap.Persistence.EfCore.Specifications
{
    internal class SpecificationEvaluator : ISpecificationEvaluator
    {
        public static readonly SpecificationEvaluator Default = new();

        public IQueryable<TEntity> Evaluate<TEntity>(IQueryable<TEntity> query, ISpecification<TEntity> specification, bool evaluateCriteriaOnly = false)
            where TEntity : class
        {
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Optimization: If only criteria evaluation is needed (for Count/Exists),
            // skip applying includes, ordering, and paging as they don't affect the result
            // and would cause unnecessary work (especially includes).
            if (evaluateCriteriaOnly)
            {
                return query;
            }

            // Apply includes (Eager Loading using Include)
            // Iterates through all include expressions defined in the specification.
            // Aggregate is used to chain the .Include() calls sequentially onto the query.
            // Example: query.Include(o => o.Customer).Include(o => o.OrderItems)
            query = specification.Includes.Aggregate(query,
                (current, include) => current.Include(include));

            var specificationResult = specification.QueryOptions?.Execute(query);
            if (specificationResult != null)
            {
                query = (IQueryable<TEntity>)specificationResult.Items;
            }

            // Return the fully constructed IQueryable
            // This IQueryable represents the complete query defined by the specification,
            // ready to be executed against the database (e.g., by calling ToListAsync, FirstOrDefaultAsync).
            return query;
        }
    }
}
