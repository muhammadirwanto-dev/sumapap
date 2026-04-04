using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Paging;

namespace Sumapap.Queries
{
    public sealed class QueryResult<T>(
        IEnumerable<T> items,
        int totalDataCount = 0,
        PageInfo? pageInfo = null) : IQueryResult<T>
    {
        public IEnumerable<T> Items { get; } = items;

        public int TotalDataCount { get; } = totalDataCount;

        /// <summary>
        /// Cursor pagination metadata (null for offset paging).
        /// </summary>
        public PageInfo? PageInfo { get; } = pageInfo;

        public QueryResult()
            : this([], 0, null)
        {
        }

        public QueryResult(int totalDataCount)
            : this([], totalDataCount, null)
        {
        }
    }
}
