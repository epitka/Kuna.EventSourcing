using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;

namespace Kuna.EventSourcing.Core.Tests;

public partial class TestAggregate : Aggregate<Guid, TestAggregate.State>
{
    public new void RaiseEvent(IAggregateEvent aggregateEvent)
    {
        base.RaiseEvent(aggregateEvent);
    }
}