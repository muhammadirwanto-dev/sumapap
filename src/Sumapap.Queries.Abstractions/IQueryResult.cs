using Sumapap.Queries.Abstractions.Paging;

namespace Sumapap.Queries.Abstractions
{
    public interface IQueryResult<out T>
    {
        IEnumerable<T> Items { get; }

        int TotalDataCount { get; }

        PageInfo? PageInfo { get; }
    }
}
