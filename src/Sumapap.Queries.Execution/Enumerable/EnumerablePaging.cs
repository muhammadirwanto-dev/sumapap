using Sumapap.Queries.Execution.Cursor;

namespace Sumapap.Queries.Execution.Enumerable
{
    internal static class EnumerablePaging
    {
        public static QueryResult<T> Apply<T>(
            IEnumerable<T> source,
            Query query)
        {
            if (query.UsesCursorPaging)
            {
                return CursorPaging.Apply(source, query);
            }

            var total = source.Count();

            if (query.UsesOffsetPaging)
            {
                var page = query.OffsetPaging!;
                var items = source
                    .Skip(page.Offset)
                    .Take(page.PageSize)
                    .ToList();

                return new QueryResult<T>(items, total);
            }

            return new QueryResult<T>([.. source], total);
        }
    }
}
