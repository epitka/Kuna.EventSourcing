using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Tests;

public partial class TestAggregate : Aggregate<TestAggregate.State>
{
    public new void RaiseEvent(IEvent @event)
    {
        base.RaiseEvent(@event);
    }
}