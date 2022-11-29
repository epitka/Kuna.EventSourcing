using Senf.EventSourcing.Core.Tests.Events;
namespace Senf.EventSourcing.Core.Tests;

public class AggregateTest
{
    [Fact]
    public void Should_Set_Correct_Event_Version()
    {
        // Arrange
        var aggregateId = Id.Create();
        var name = Guid.NewGuid().ToString();

        var aggregate = TestAggregate.Create(aggregateId, name);

        // Act

        aggregate.Raise(new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"));

        aggregate.Raise(new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"));

        // Assert
        aggregate.Version.Should().Be(2);

        aggregate.Id.Should().Be(aggregateId);

        var events = aggregate.GetPendingEvents();

        var state = aggregate.GetState();

        state.Id.Should().Be(aggregateId);
        state.Name.Should().Be(name);
        state.ChangedValue.Should().Be("Value2");

        Assert.Collection(
            events,
            item => { item.Version.Should().Be(0); },
            item => { item.Version.Should().Be(1); },
            item => { item.Version.Should().Be(2); });
    }

    [Fact]
    public void GetState_Should_Return_Cloned_Internal_State()
    {
        var aggregate = new TestAggregate();

        var aggregateId = Id.Create();

        var created = new TestAggregateCreated(Id: aggregateId, Name: "Name1")
        {
            Version = 0,
        };

        var changed = new TestAggregateChanged(Id: aggregateId, ChangedValue: "changed1")
        {
            Version = 1,
        };

        aggregate.Raise(created);
        aggregate.Raise(changed);

        var expectedState = new TestAggregate.State()
        {
            Name = created.Name,
            ChangedValue = changed.ChangedValue,
        };

        var currentState = aggregate.GetState();

        currentState.Name.Should().Be(expectedState.Name);
        currentState.ChangedValue.Should().Be(expectedState.ChangedValue);
    }
}