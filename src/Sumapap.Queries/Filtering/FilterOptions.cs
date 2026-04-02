namespace Sumapap.Queries.Filtering
{
    public sealed class FilterOptions
    {
        public FilterGroup RootGroup { get; } = new();

        public static FilterOptions Empty => new();

        private FilterOptions()
        {
        }

        public FilterOptions(FilterGroup rootGroup)
        {
            RootGroup = rootGroup;
        }
    }
}
