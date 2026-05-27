namespace Sumapap.Queries.Abstractions.Paging
{
    /// <summary>
    /// Specifies the direction for cursor-based pagination.
    /// </summary>
    public enum CursorDirection
    {
        /// <summary>
        /// Retrieve items after the cursor (next page).
        /// </summary>
        Forward,

        /// <summary>
        /// Retrieve items before the cursor (previous page).
        /// </summary>
        Backward
    }
}
