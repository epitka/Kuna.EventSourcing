using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;
using Kuna.EventSourcing.Core.Tests.Events;

namespace Kuna.EventSourcing.Core.Tests.AggregateTests;

public class GetPendingEvents
{
    [Fact]
    public void Should_Return_Copy_Of_Events_Raised()
    {
        var aggregate = new TestAggregate();
        var aggregateId = Guid.NewGuid();

        var events = new List<object>()
        {
            new TestAggregateCreated(Id: aggregateId, Name: "asdf"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"),
        };

        aggregate.GetPendingEvents().Length.Should().Be(0);

        aggregate.RaiseEvent(events[0]);
        aggregate.GetPendingEvents().Length.Should().Be(1);

        aggregate.RaiseEvent(events[1]);
        aggregate.GetPendingEvents().Length.Should().Be(2);

        aggregate.RaiseEvent(events[2]);
        aggregate.GetPendingEvents().Length.Should().Be(3);

        var pendingEvents = aggregate.GetPendingEvents();

        for (var i = 0; i < pendingEvents.Length; i++)
        {
            var pendingEvent = pendingEvents[i];
            var @event = events[i];

            pendingEvent.Should().Be(@event);
        }
    }
}
