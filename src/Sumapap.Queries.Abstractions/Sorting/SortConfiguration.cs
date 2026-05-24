namespace Sumapap.Queries.Abstractions.Sorting
{
    public sealed class SortConfiguration
    {
        public static SortConfiguration Empty => new();

        public IList<SortDescriptor> Sorts { get; private set; } = [];

        public SortConfiguration()
        {
        }

        public SortConfiguration By(string field, SortDirection direction = SortDirection.Asc)
        {
            Sorts = [new SortDescriptor(field, direction)];

            return this;
        }

        public SortConfiguration ThenBy(string field, SortDirection direction = SortDirection.Asc)
        {
            Sorts.Add(new SortDescriptor(field, direction));

            return this;
        }
    }
}
