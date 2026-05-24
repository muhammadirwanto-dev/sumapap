using System.Collections.Concurrent;
using Sumapap.Ddd.Abstractions;

namespace Sumapap.Ddd
{
    public abstract class DomainEntity
    {
        private readonly ConcurrentQueue<IDomainEvent> _events = [];

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _events.Enqueue(domainEvent);
        }

        public IReadOnlyList<IDomainEvent> ConsumeEvents()
        {
            var events = _events.ToArray();

            _events.Clear();

            return events;
        }

        public IReadOnlyList<IDomainEvent> GetEvents() => [.. _events];

        public void ClearEvents() => _events.Clear();
    }
}
