using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sumapap.Ddd.Abstractions
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
