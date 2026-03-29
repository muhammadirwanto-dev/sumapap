using Mediator;
using Sumapap.Ddd.Abstractions;

namespace Sumapap.Ddd.Mediator.Abstractions
{
    public interface IDomainEventAdapter : IDomainEvent, INotification;
}
