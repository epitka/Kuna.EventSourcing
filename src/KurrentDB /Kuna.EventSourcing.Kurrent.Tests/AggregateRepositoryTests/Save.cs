using FakeItEasy.Configuration;
using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers;

namespace Kuna.EventSourcing.Kurrent.Tests.AggregateRepositoryTests;

public class Save
{
    [Fact]
    public async Task Should_Dequeue_Pending_Events_On_Aggregate()
    {
        var fakeWriter = A.Fake<IStreamWriter>();
        var repository = new TestAggregateRepository(A.Fake<IStreamReader>(), fakeWriter);
        var aggregateId = Guid.NewGuid();

        var aggregate = TestAggregate.Create(aggregateId, "test");

        // check pre-condition
        aggregate.GetPendingEvents().Length.Should().Be(1);

        await repository.Save(aggregate, CancellationToken.None);

        aggregate.GetPendingEvents().Should().BeEmpty();
    }

    [Fact]
    public async Task When_No_Pending_Events_Should_Not_Attempt_Write()
    {
        var fakeWriter = A.Fake<IStreamWriter>(opt => opt.Strict());

        var repository = new TestAggregateRepository(A.Fake<IStreamReader>(), fakeWriter);
        var aggregateId = Guid.NewGuid();

        var aggregate = TestAggregate.Create(aggregateId, "test");

        // let's force dequeue events
        aggregate.DequeuePendingEvents();

        // check pre-condition
        aggregate.GetPendingEvents().Length.Should().Be(0);

        await repository.Save(aggregate, CancellationToken.None);

        // since we use Strict option on fake and we did not specify
        // any calls to any method on fake would fail test but just for explicitness
        A.CallTo(fakeWriter)
         .WithAnyArguments()
         .MustNotHaveHappened();
    }

    [Fact]
    public async Task Should_Call_Writer_With_Correct_Arguments()
    {
        var aggregateId = Guid.NewGuid();
        var aggregate = TestAggregate.Create(aggregateId, "test");

        aggregate.ChangeName("newName");

        var pendingEvents = aggregate.GetPendingEvents();

        ArgumentCollection? callArguments = null;
        var fakeWriter = A.Fake<IStreamWriter>();

        // check pre-condition
        pendingEvents .Length.Should().Be(2);

        A.CallTo(fakeWriter)
         .WithReturnType<Task>()
         .Where(x => x.Method.Name == nameof(fakeWriter.Write))
         .WithAnyArguments()
         .Invokes(x => callArguments = x.Arguments);

        var repository = new TestAggregateRepository(A.Fake<IStreamReader>(), fakeWriter);
        await repository.Save(aggregate, CancellationToken.None);

        callArguments.Should().NotBeNull();
        callArguments!.Count.Should().Be(4);
        var capturedStreamId = (string)callArguments[0]!;
        var capturedVersion = callArguments[1]!;
        var capturedEvents = (IEnumerable<object>)callArguments[2]!;
        callArguments[3]!.Should().BeOfType<CancellationToken>();

        capturedStreamId.Should().Be(repository.StreamPrefix + aggregateId);
        capturedVersion.Should().Be(aggregate.OriginalVersion);

        for (var i = 0; i < pendingEvents.Length; i++)
        {
            var pendingEvent = pendingEvents[i];
            var capturedEvent = capturedEvents.ElementAt(i);
            capturedEvent.Should().Be(pendingEvent);
        }
    }
}
