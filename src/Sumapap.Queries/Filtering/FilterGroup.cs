namespace Sumapap.Queries.Filtering
{
    public sealed class FilterGroup(CompositeOperator @operator)
    {
        public CompositeOperator Operator { get; } = @operator;

        public List<FilterDescriptor> Filters { get; } = [];

        public List<FilterGroup> SubGroups { get; } = [];
    }
}
