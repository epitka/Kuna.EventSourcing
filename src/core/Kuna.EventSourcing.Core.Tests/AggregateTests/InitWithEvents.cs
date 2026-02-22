using DeepEqual.Syntax;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.TestKit;

namespace Kuna.EventSourcing.Core.Tests.AggregateTests;

public class InitWithEvents
{
    [Fact]
    public void Should_Initialize_Aggregate()
    {
        var aggregate = new TestAggregate();
        var aggregateId = Guid.NewGuid();
        var lastChangedValue = "Last";
        var name = "name";

        var events = new List<object>()
        {
            new TestAggregateCreated(Id: aggregateId, Name: "name"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue:lastChangedValue),
        };

        aggregate.InitWith(events);

        var expectedState = new TestAggregate.State();
        expectedState.SetId(aggregateId);
        expectedState.Name = name;
        expectedState.ChangedValue = lastChangedValue;

        // since InitWith is used to initialize state of the aggregate
        // from stream of events, after completion of initialization
        // version of the aggregate matches original version.
        // further changes to aggregate will increase version, and original version will not
        expectedState.Version = (ulong?)(events.Count - 1);
        expectedState.OriginalVersion = (ulong?)(events.Count - 1);

        var currentState = aggregate.CurrentState.DeepClone();

        expectedState.ShouldDeepEqual(currentState);
    }

    [Fact]
    public void When_State_Already_Mutated_Should_Throw()
    {
        var aggregate = new TestAggregate();
        var aggregateId = Guid.NewGuid();

        var events = new List<object>()
        {
            new TestAggregateCreated(Id: aggregateId, Name: "name"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"),
        };

        // init state by raising at least one event
        // this would also be true if InitWith events or state was called
        aggregate.RaiseEvent(events[0]);

        // check pre-condition
        aggregate.Version.HasValue.Should().BeTrue();

        Assert.Throws<InvalidOperationException>(() => aggregate.InitWith(events));
    }
}
