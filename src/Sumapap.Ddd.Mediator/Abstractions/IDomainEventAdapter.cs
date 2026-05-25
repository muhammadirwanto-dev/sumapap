using Mediator;
using Sumapap.Ddd.Abstractions.Events;

namespace Sumapap.Ddd.Mediator.Abstractions
{
    public interface IDomainEventAdapter : IDomainEvent, INotification;
}
