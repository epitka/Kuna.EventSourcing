using DeepEqual.Syntax;
using Senf.EventSourcing.Core.Exceptions;
using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests.AggregateStateTests;

public class ApplyEvent
{
    [Fact]
    public void When_Event_Version_Is_Not_Increment_Of_Current_Version_Should_Throw()
    {
        var state = new TestAggregate.State();

        var @event = new TestAggregateChanged(Id: Id.Create(), ChangedValue: "test")
        {
            Version = state.Version + 2,
        };

        Assert.Throws<InvalidExpectedVersionException>(() => state.ApplyEvent(@event));
    }

    [Fact]
    public void Should_Invoke_Correct_State_Mutation_Apply_Method()
    {
        var state = new TestAggregate.State();
        var aggregateId = Id.Create();
        var name = Guid.NewGuid().ToString();

        var @event = new TestAggregateCreated(Id: aggregateId, Name: name )
        {
            Version = 0,
        };

        state.ApplyEvent(@event);

        var expectedState = new TestAggregate.State();
        expectedState.SetId(aggregateId);
        expectedState.Version = 0;
        expectedState.Name = name;

        expectedState.ShouldDeepEqual(state);
    }

    [Fact]
    public void Should_Increment_Version()
    {
    }
}
