using DeepEqual.Syntax;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests.AggregateTests;

public class InitWithState
{
    [Fact]
    public void Should_Initialize_Aggregate()
    {
        var aggregate = new TestAggregate();
        var aggregateId = Guid.NewGuid();

        const string lastChangedValue = "Last";
        const string name = "name";

        var expectedState = new TestAggregate.State();
        expectedState.SetId(aggregateId);
        expectedState.Name = name;
        expectedState.ChangedValue = lastChangedValue;
        expectedState.Version = 7;
        expectedState.OriginalVersion = 2;

        ((IAggregate<TestAggregate.State>)aggregate).InitWithState(expectedState);

        var currentState = aggregate.GetState();

        expectedState.ShouldDeepEqual(currentState);
    }

    [Fact]
    public void When_State_Already_Mutated_Should_Throw()
    {
        var aggregate = new TestAggregate();
        var aggregateId = Guid.NewGuid();
        var name = "name";
        var expectedState = new TestAggregate.State();
        expectedState.SetId(aggregateId);
        expectedState.Name = name;
        expectedState.Version = 7;
        expectedState.OriginalVersion = 2;

        // init state by raising at least one event
        // this would also be true if InitWith events or state was called
        aggregate.RaiseEvent(new TestAggregateCreated(Guid.NewGuid(), string.Empty));

        // check pre-condition
        aggregate.Version.Should().BeGreaterThan(-1);

        Assert.Throws<InvalidOperationException>(() => ((IAggregate<TestAggregate.State>) aggregate).InitWithState(expectedState));
    }
}
