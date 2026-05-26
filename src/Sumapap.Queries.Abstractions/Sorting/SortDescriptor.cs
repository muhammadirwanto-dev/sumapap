namespace Sumapap.Queries.Abstractions.Sorting
{
    /// <summary>
    /// Represents a single sort criterion with a field name and direction.
    /// </summary>
    public sealed class SortDescriptor(
        string field,
        SortDirection direction = SortDirection.Asc)
    {
        /// <summary>
        /// Gets the name of the field to sort by.
        /// </summary>
        public string Field { get; } = field;

        /// <summary>
        /// Gets the sort direction (ascending or descending).
        /// </summary>
        public SortDirection Direction { get; } = direction;
    }
}
