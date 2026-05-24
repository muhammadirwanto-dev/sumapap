namespace Sumapap.Queries.Abstractions.Paging
{
    public sealed class CursorPaginationConfiguration(
        string cursorField,
        string? cursor = null,
        int limit = 20,
        CursorDirection direction = CursorDirection.Forward)
    {
        /// <summary>
        /// Opaque cursor value (consumer-defined encoding).
        /// </summary>
        public string? Cursor { get; } = cursor;

        /// <summary>
        /// Field used for cursor comparison (must match sorting).
        /// </summary>
        public string CursorField { get; } = cursorField;

        public int Limit { get; } = limit;

        public CursorDirection Direction { get; } = direction;
    }
}
