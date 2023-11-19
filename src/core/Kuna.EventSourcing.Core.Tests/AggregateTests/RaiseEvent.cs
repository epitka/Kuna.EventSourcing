using DeepEqual.Syntax;

namespace Kuna.EventSourcing.Core.Tests.AggregateTests;

public class RaiseEvent
{
    [Fact]
    public void Should_Increment_Version()
    {
        // Arrange
        var aggregateId = Id.Create();
        var name = Guid.NewGuid().ToString();

        // Act
        var aggregate = new TestAggregate();

        aggregate.RaiseEvent(new TestAggregateCreated(Id: aggregateId, Name: name));

        aggregate.RaiseEvent(new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"));

        aggregate.RaiseEvent(new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"));

        // Assert
        aggregate.Version.Should().Be(2);
    }

    [Fact]
    public void Should_Apply_StateChanges()
    {
        // Arrange
        var aggregateId = Id.Create();
        var name = Guid.NewGuid().ToString();

        // Act
        var aggregate = new TestAggregate();
        aggregate.RaiseEvent(new TestAggregateCreated(Id: aggregateId, Name: name));
        aggregate.RaiseEvent(new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"));
        aggregate.RaiseEvent(new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"));

        // Assert
        var state = aggregate.GetState();

        var expectedState = new TestAggregate.State();
        expectedState.Name = name;
        expectedState.ChangedValue = "Value2";
        expectedState.SetId(aggregateId);
        expectedState.Version = 2;

        expectedState.ShouldDeepEqual(state);

    }

    [Fact]
    public void Should_Enqueue_Events()
    {
        // Arrange
        var aggregateId = Id.Create();
        var name = Guid.NewGuid().ToString();

        // Act
        var aggregate = new TestAggregate();

        var events = new List<object>()
        {
            new TestAggregateCreated(Id: aggregateId, Name: name),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value1"),
            new TestAggregateChanged(Id: aggregateId, ChangedValue: "Value2"),
        };

        aggregate.RaiseEvent(events[0]);
        aggregate.RaiseEvent(events[1]);
        aggregate.RaiseEvent(events[2]);

        // Assert
        var pendingEvents = aggregate.GetPendingEvents();

        Assert.Collection(
            pendingEvents,
            item =>
            {
                item.Should().Be(events[0]);
            },
            item =>
            {
                item.Should().Be(events[1]);
            },
            item =>
            {
                item.Should().Be(events[2]);
            });
    }
}
