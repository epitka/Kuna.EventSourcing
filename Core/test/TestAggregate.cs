using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Tests;

public partial class TestAggregate : Aggregate<Guid, TestAggregate.State>
{
    public new void RaiseEvent(IAggregateEvent aggregateEvent)
    {
        base.RaiseEvent(aggregateEvent);
    }
}