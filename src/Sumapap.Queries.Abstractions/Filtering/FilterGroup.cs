namespace Sumapap.Queries.Abstractions.Filtering
{
    /// <summary>
    /// Groups multiple filters with a composite operator (AND/OR).
    /// </summary>
    public class FilterGroup
    {
        /// <summary>
        /// Gets the operator used to combine filters in this group.
        /// </summary>
        public CompositeOperator Operator { get; private set; } = CompositeOperator.And;

        /// <summary>
        /// Gets the individual filters in this group.
        /// </summary>
        public IEnumerable<FilterDescriptor> Filters { get; private set; } = [];

        /// <summary>
        /// Gets the nested filter subgroups.
        /// </summary>
        public IEnumerable<FilterConfiguration> SubGroups { get; private set; } = [];

        /// <summary>
        /// Sets the composite operator for this group.
        /// </summary>
        /// <param name="operator">The operator to use (And or Or).</param>
        /// <returns>This instance for method chaining.</returns>
        public FilterGroup WithOperator(CompositeOperator @operator)
        {
            Operator = @operator;

            return this;
        }

        /// <summary>
        /// Sets the filters for this group.
        /// </summary>
        /// <param name="filters">The list of filters.</param>
        /// <returns>This instance for method chaining.</returns>
        public FilterGroup WithFilters(List<FilterDescriptor> filters)
        {
            Filters = filters;

            return this;
        }

        /// <summary>
        /// Sets the nested subgroups for this group.
        /// </summary>
        /// <param name="subGroups">The list of subgroups.</param>
        /// <returns>This instance for method chaining.</returns>
        public FilterGroup HavingSubGroups(List<FilterConfiguration> subGroups)
        {
            SubGroups = subGroups;

            return this;
        }
    }
}
