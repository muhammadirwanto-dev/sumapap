namespace Sumapap.Queries.Sorting
{
    public sealed class SortOptions
    {
        public static SortOptions Empty => new();

        public IList<SortDescriptor> Sorts { get; private set; } = [];

        private SortOptions()
        {
        }

        public SortOptions(IList<SortDescriptor> sorts)
        {
            Sorts = sorts;
        }

        public SortOptions By(string field, SortDirection direction = SortDirection.Asc)
        {
            Sorts = [new SortDescriptor(field, direction)];

            return this;
        }

        public SortOptions ThenBy(string field, SortDirection direction = SortDirection.Asc)
        {
            Sorts.Add(new SortDescriptor(field, direction));

            return this;
        }
    }
}
