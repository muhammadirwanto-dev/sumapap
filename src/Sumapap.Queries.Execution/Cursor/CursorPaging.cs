using System.Collections;
using Sumapap.Queries.Execution.Internals;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Execution.Cursor
{
    internal static class CursorPaging
    {
        public static QueryResult<T> Apply<T>(
            IEnumerable<T> source,
            Query query)
        {
            var paging = query.CursorPaging!;
            var prop = ReflectionCache.GetProperty<T>(paging.CursorField)
                ?? throw new InvalidOperationException(
                    $"Cursor field '{paging.CursorField}' not found on '{typeof(T).Name}'.");

            if (!string.IsNullOrEmpty(paging.Cursor))
            {
                var cursorValue = CursorEncryption.DecodeCursor(
                    paging.Cursor,
                    prop.PropertyType);

                source = paging.Direction == CursorDirection.Forward
                    ? source.Where(x =>
                        Comparer.Default.Compare(prop.GetValue(x), cursorValue) > 0)
                    : source.Where(x =>
                        Comparer.Default.Compare(prop.GetValue(x), cursorValue) < 0);
            }

            var items = source.Take(paging.Limit + 1).ToList();
            var hasNext = items.Count > paging.Limit;
            var result = items.Take(paging.Limit).ToList();

            var endCursor = result.Count > 0
                ? CursorEncryption.EncodeCursor(
                    prop.GetValue(result.Last())!)
                : null;

            return new QueryResult<T>(
                result,
                totalDataCount: -1,
                pageInfo: new PageInfo(
                    hasNextPage: hasNext,
                    hasPreviousPage: paging.Cursor != null,
                    startCursor: paging.Cursor,
                    endCursor: endCursor
                )
            );
        }
    }
}
