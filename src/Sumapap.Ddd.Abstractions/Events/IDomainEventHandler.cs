namespace Sumapap.Ddd.Abstractions.Events
{
    public interface IDomainEventHandler<in TEvent>
        where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
