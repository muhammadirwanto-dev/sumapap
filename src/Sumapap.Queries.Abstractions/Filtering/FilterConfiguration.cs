namespace Sumapap.Queries.Abstractions.Filtering
{
    /// <summary>
    /// Root container for filter logic, combining multiple filters and subgroups.
    /// </summary>
    public sealed class FilterConfiguration : FilterGroup
    {
        /// <summary>
        /// Gets an empty filter configuration with no filters.
        /// </summary>
        public static FilterConfiguration Empty => new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterConfiguration"/> class.
        /// </summary>
        public FilterConfiguration()
        {
        }
    }
}
