namespace Sumapap.Queries.Filtering
{
    public sealed class FilterGroup
    {
        public CompositeOperator Operator { get; private set; } = CompositeOperator.And;

        public IEnumerable<FilterDescriptor> Filters { get; private set; } = [];

        public IEnumerable<FilterGroup> SubGroups { get; private set; } = [];

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

        public FilterGroup HasSubGroups(List<FilterGroup> subGroups)
        {
            SubGroups = subGroups;

            return this;
        }
    }
}
