namespace Sumapap.Queries.Abstractions.Filtering
{
    public sealed class FilterConfiguration : FilterGroup
    {
        public static FilterConfiguration Empty => new();

        public FilterConfiguration()
        {
        }
    }
}
