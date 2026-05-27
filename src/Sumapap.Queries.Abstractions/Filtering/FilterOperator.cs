namespace Sumapap.Queries.Abstractions.Filtering
{
    /// <summary>
    /// Specifies the comparison operation for a filter.
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>
        /// Tests for equality.
        /// </summary>
        Equals,

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        NotEquals,

        /// <summary>
        /// Tests if the field value is greater than the specified value.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Tests if the field value is greater than or equal to the specified value.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Tests if the field value is less than the specified value.
        /// </summary>
        LessThan,

        /// <summary>
        /// Tests if the field value is less than or equal to the specified value.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Tests if the field value contains the specified substring (for strings).
        /// </summary>
        Contains,

        /// <summary>
        /// Tests if the field value starts with the specified substring (for strings).
        /// </summary>
        StartsWith,

        /// <summary>
        /// Tests if the field value ends with the specified substring (for strings).
        /// </summary>
        EndsWith,

        /// <summary>
        /// Tests if the field value is in the specified collection.
        /// </summary>
        In
    }
}
