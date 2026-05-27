namespace Sumapap.Queries.Abstractions.Filtering
{
    /// <summary>
    /// Specifies how filters within a group are combined.
    /// </summary>
    public enum CompositeOperator
    {
        /// <summary>
        /// All filters in the group must match (logical AND).
        /// </summary>
        And,

        /// <summary>
        /// At least one filter in the group must match (logical OR).
        /// </summary>
        Or
    }
}
