namespace Sumapap.Queries.Filtering
{
    public sealed class FilterOptions
    {
        public IReadOnlyList<FilterDescriptor> Filters { get; } = [];

        public static FilterOptions Empty => new();

        public FilterOptions()
        {
        }

        public FilterOptions(IReadOnlyList<FilterDescriptor> filters)
        {
            Filters = filters;
        }
    }
}
