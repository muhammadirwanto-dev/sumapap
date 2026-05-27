namespace Sumapap.Queries.Abstractions.Filtering
{
    /// <summary>
    /// Represents a single filter condition with a field name, operator, and value.
    /// </summary>
    public sealed class FilterDescriptor(string field, FilterOperator @operator, object? value = null)
    {
        /// <summary>
        /// Gets the name of the field to filter on.
        /// </summary>
        public string Field { get; } = field;

        /// <summary>
        /// Gets the filter operator to apply.
        /// </summary>
        public FilterOperator Operator { get; } = @operator;

        /// <summary>
        /// Gets the value to compare against, or null for operators like IsNull.
        /// </summary>
        public object? Value { get; } = value;
    }
}
