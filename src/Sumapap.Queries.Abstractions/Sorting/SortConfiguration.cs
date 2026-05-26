namespace Sumapap.Queries.Abstractions.Sorting
{
    /// <summary>
    /// Contains multiple sort descriptors for multi-level sorting.
    /// </summary>
    public sealed class SortConfiguration
    {
        /// <summary>
        /// Gets an empty sort configuration with no sort criteria.
        /// </summary>
        public static SortConfiguration Empty => new();

        /// <summary>
        /// Gets the list of sort descriptors, in order of precedence.
        /// </summary>
        public IList<SortDescriptor> Sorts { get; private set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="SortConfiguration"/> class.
        /// </summary>
        public SortConfiguration()
        {
        }

        /// <summary>
        /// Sets the primary sort field and direction.
        /// </summary>
        /// <param name="field">The field name to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        /// <returns>This instance for method chaining.</returns>
        public SortConfiguration By(string field, SortDirection direction = SortDirection.Asc)
        {
            Sorts = [new SortDescriptor(field, direction)];

            return this;
        }

        /// <summary>
        /// Adds an additional sort field for multi-level sorting.
        /// </summary>
        /// <param name="field">The field name to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        /// <returns>This instance for method chaining.</returns>
        public SortConfiguration ThenBy(string field, SortDirection direction = SortDirection.Asc)
        {
            Sorts.Add(new SortDescriptor(field, direction));

            return this;
        }
    }
}
