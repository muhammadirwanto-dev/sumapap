using Sumapap.Ddd.Abstractions;

namespace Sumapap.Ddd.Tests
{
    public class DomainEntityTests
    {
        private record TestDomainEvent(string Message) : IDomainEvent;

        private class TestEntity : DomainEntity
        {
            public void RaiseEvent(IDomainEvent domainEvent)
            {
                AddDomainEvent(domainEvent);
            }
        }

        [Fact]
        public void AddDomainEvent_WhenCalled_AddsEventToCollection()
        {
            // Arrange
            var entity = new TestEntity();
            var domainEvent = new TestDomainEvent("Test event");

            // Act
            entity.RaiseEvent(domainEvent);

            // Assert
            var events = entity.GetEvents();
            Assert.Single(events);
            Assert.Equal(domainEvent, events[0]);
        }

        [Fact]
        public void GetEvents_WhenNoEvents_ReturnsEmptyCollection()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            var events = entity.GetEvents();

            // Assert
            Assert.Empty(events);
        }

        [Fact]
        public void GetEvents_WhenMultipleEvents_ReturnsAllEvents()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestDomainEvent("Event 1");
            var event2 = new TestDomainEvent("Event 2");
            var event3 = new TestDomainEvent("Event 3");

            // Act
            entity.RaiseEvent(event1);
            entity.RaiseEvent(event2);
            entity.RaiseEvent(event3);

            // Assert
            var events = entity.GetEvents();
            Assert.Equal(3, events.Count);
            Assert.Equal(event1, events[0]);
            Assert.Equal(event2, events[1]);
            Assert.Equal(event3, events[2]);
        }

        [Fact]
        public void ConsumeEvents_WhenCalled_ReturnsEventsAndClearsCollection()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestDomainEvent("Event 1");
            var event2 = new TestDomainEvent("Event 2");
            entity.RaiseEvent(event1);
            entity.RaiseEvent(event2);

            // Act
            var consumedEvents = entity.ConsumeEvents();

            // Assert
            Assert.Equal(2, consumedEvents.Count);
            Assert.Equal(event1, consumedEvents[0]);
            Assert.Equal(event2, consumedEvents[1]);
            Assert.Empty(entity.GetEvents());
        }

        [Fact]
        public void ConsumeEvents_WhenCalledTwice_SecondCallReturnsEmpty()
        {
            // Arrange
            var entity = new TestEntity();
            var domainEvent = new TestDomainEvent("Test event");
            entity.RaiseEvent(domainEvent);

            // Act
            var firstConsume = entity.ConsumeEvents();
            var secondConsume = entity.ConsumeEvents();

            // Assert
            Assert.Single(firstConsume);
            Assert.Empty(secondConsume);
        }

        [Fact]
        public void ClearEvents_WhenCalled_RemovesAllEvents()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestDomainEvent("Event 1");
            var event2 = new TestDomainEvent("Event 2");
            entity.RaiseEvent(event1);
            entity.RaiseEvent(event2);

            // Act
            entity.ClearEvents();

            // Assert
            Assert.Empty(entity.GetEvents());
        }

        [Fact]
        public void DomainEntity_ThreadSafety_HandlesParallelAdditions()
        {
            // Arrange
            var entity = new TestEntity();
            var eventCount = 100;

            // Act
            Parallel.For(0, eventCount, i =>
            {
                entity.RaiseEvent(new TestDomainEvent($"Event {i}"));
            });

            // Assert
            var events = entity.GetEvents();
            Assert.Equal(eventCount, events.Count);
        }

        [Fact]
        public void GetEvents_DoesNotConsumeEvents_EventsStillAvailable()
        {
            // Arrange
            var entity = new TestEntity();
            var domainEvent = new TestDomainEvent("Test event");
            entity.RaiseEvent(domainEvent);

            // Act
            var firstGet = entity.GetEvents();
            var secondGet = entity.GetEvents();

            // Assert
            Assert.Single(firstGet);
            Assert.Single(secondGet);
            Assert.Equal(domainEvent, firstGet[0]);
            Assert.Equal(domainEvent, secondGet[0]);
        }
    }
}
