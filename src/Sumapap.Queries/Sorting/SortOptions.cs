namespace Sumapap.Queries.Sorting
{
    public sealed class SortOptions
    {
        public IReadOnlyList<SortDescriptor> Sorts { get; } = [];

        public static SortOptions Empty => new();

        public static SortOptions By(string field, SortDirection direction = SortDirection.Asc)
            => new(
            [
                new SortDescriptor(field, direction),
            ]);

        public SortOptions()
        {
        }

        public SortOptions(IReadOnlyList<SortDescriptor> sorts)
        {
            Sorts = sorts;
        }
    }
}
