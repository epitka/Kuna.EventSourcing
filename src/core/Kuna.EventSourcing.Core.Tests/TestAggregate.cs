using Kuna.EventSourcing.Core.Aggregates;


namespace Kuna.EventSourcing.Core.Tests;

public partial class TestAggregate : Aggregate<Guid, TestAggregate.State>
{
    public new void RaiseEvent(object aggregateEvent)
    {
        base.RaiseEvent(aggregateEvent);
    }
}