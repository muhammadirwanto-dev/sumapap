namespace Sumapap.Queries.Filtering
{
    public class FilterGroup
    {
        public CompositeOperator Operator { get; private set; } = CompositeOperator.And;

        public IEnumerable<FilterDescriptor> Filters { get; private set; } = [];

        public IEnumerable<FilterConfiguration> SubGroups { get; private set; } = [];

        public FilterGroup WithOperator(CompositeOperator @operator)
        {
            Operator = @operator;

            return this;
        }

        public FilterGroup WithFilters(List<FilterDescriptor> filters)
        {
            Filters = filters;

            return this;
        }

        public FilterGroup HavingSubGroups(List<FilterConfiguration> subGroups)
        {
            SubGroups = subGroups;

            return this;
        }
    }
}
