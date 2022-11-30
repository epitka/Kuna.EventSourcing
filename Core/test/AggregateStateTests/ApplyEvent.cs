using DeepEqual.Syntax;
using Senf.EventSourcing.Core.Exceptions;
using Senf.EventSourcing.Core.Tests.Events;

namespace Senf.EventSourcing.Core.Tests.AggregateStateTests;

public class ApplyEvent
{

    [Fact]
    public void Should_Invoke_Correct_State_Mutation_Apply_Method()
    {
        var state = new TestAggregate.State();
        var aggregateId = Id.Create();
        var name = Guid.NewGuid().ToString();

        var @event = new TestAggregateCreated(Id: aggregateId, Name: name);

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
