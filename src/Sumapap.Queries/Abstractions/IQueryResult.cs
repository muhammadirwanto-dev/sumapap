using Sumapap.Queries.Paging;

namespace Sumapap.Queries.Abstractions
{
    public interface IQueryResult<out T>
    {
        IReadOnlyList<T> Items { get; }

        int TotalDataCount { get; }

        PageInfo? PageInfo { get; }
    }
}
