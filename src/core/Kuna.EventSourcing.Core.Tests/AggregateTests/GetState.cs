using DeepEqual.Syntax;

namespace Kuna.EventSourcing.Core.Tests.AggregateTests;

public class GetState
{
    [Fact]
    public void Should_Return_Cloned_Internal_State()
    {
        var aggregate = new TestAggregate();

        var aggregateId = Id.Create();

        var created = new TestAggregateCreated(Id: aggregateId, Name: "Name1");

        var changed = new TestAggregateChanged(Id: aggregateId, ChangedValue: "changed1");

        aggregate.RaiseEvent(created);
        aggregate.RaiseEvent(changed);

        var expectedState = new TestAggregate.State()
        {
            Name = created.Name,
            ChangedValue = changed.ChangedValue,
            Version = 1,
            OriginalVersion = -1    
        };
        expectedState.SetId(aggregateId);

        var currentState = aggregate.GetState();

        currentState.ShouldDeepEqual(expectedState);
    }
}
