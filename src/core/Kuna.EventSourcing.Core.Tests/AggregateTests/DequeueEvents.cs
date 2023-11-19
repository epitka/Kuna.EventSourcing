using AutoFixture;

namespace Kuna.EventSourcing.Core.Tests.AggregateTests;

public class DequeueEvents
{
    private static readonly IFixture Fixture = new Fixture();

    [Fact]
    public void Should_Clear_Internal_Queue_Of_Pending_Events()
    {
        var sut = new TestAggregate();

        // this is just for testing, normally each event
        // would have same aggregate id
        sut.RaiseEvent(Fixture.Create<TestAggregateCreated>());
        sut.RaiseEvent(Fixture.Create<TestAggregateChanged>());

        // pre-condition
        var pendingEvents = sut.GetPendingEvents();
        pendingEvents.Length.Should().Be(2);

        sut.DequeuePendingEvents();

        pendingEvents = sut.GetPendingEvents();

        pendingEvents.Length.Should().Be(0);
    }
}
