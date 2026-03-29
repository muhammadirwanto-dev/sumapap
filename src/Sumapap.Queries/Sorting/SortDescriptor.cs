namespace Sumapap.Queries.Sorting
{
    public sealed class SortDescriptor(
        string field,
        SortDirection direction = SortDirection.Asc)
    {
        public string Field { get; } = field;

        public SortDirection Direction { get; } = direction;
    }
}
