using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests.AggregateTests;

public class GetPendingEvents
{
    [Fact]
    public void Should_Return_Copy_Of_Events_Raised()
    {
        var aggregate = new TestAggregate();
        var aggregateId = Guid.NewGuid();

        var events = new List<IEvent>()
        {
            new TestAggregateCreated(Id: aggregateId, Name: "asdf"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"),
        };

        aggregate.GetPendingEvents().Count().Should().Be(0);

        aggregate.RaiseEvent(events[0]);
        aggregate.GetPendingEvents().Count().Should().Be(1);

        aggregate.RaiseEvent(events[1]);
        aggregate.GetPendingEvents().Count().Should().Be(2);

        aggregate.RaiseEvent(events[2]);
        aggregate.GetPendingEvents().Count().Should().Be(3);
    }
}
