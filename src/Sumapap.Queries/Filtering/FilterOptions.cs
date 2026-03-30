namespace Sumapap.Queries.Filtering
{
    public sealed class FilterOptions
    {
        public FilterGroup RootGroup { get; } = new(CompositeOperator.And);

        public static FilterOptions Empty => new();

        public FilterOptions()
        {
        }

        public FilterOptions(FilterGroup rootGroup)
        {
            RootGroup = rootGroup;
        }
    }
}
