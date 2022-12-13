﻿using EventStore.Client;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

namespace Senf.EventSourcing.Core.EventStore.Tests.AggregateRepositoryTests;

public class Save
{
    [Fact]
    public async Task Should_Dequeue_Pending_Events_On_Aggregate()
    {
        var fakeWriter = A.Fake<IAggregateStreamWriter>();
        var repository = new TestAggregateRepository(A.Fake<IAggregateStreamReader>(), fakeWriter);
        var aggregateId = Guid.NewGuid();

        var aggregate = TestAggregate.Create(aggregateId, "test");

        // check pre-condition
        aggregate.GetPendingEvents().Length.Should().Be(1);

        await repository.Save(aggregate, default);

        aggregate.GetPendingEvents().Should().BeEmpty();
    }

    [Fact]
    public async Task When_No_Pending_Events_Should_Not_Attempt_Write()
    {
        var fakeWriter = A.Fake<IAggregateStreamWriter>(opt => opt.Strict());

        var repository = new TestAggregateRepository(A.Fake<IAggregateStreamReader>(), fakeWriter);
        var aggregateId = Guid.NewGuid();

        var aggregate = TestAggregate.Create(aggregateId, "test");

        // let's force dequeue events
        aggregate.DequeuePendingEvents();

        // check pre-condition
        aggregate.GetPendingEvents().Length.Should().Be(0);

        await repository.Save(aggregate, default);

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
        var fakeWriter = A.Fake<IAggregateStreamWriter>();

        // check pre-condition
        pendingEvents .Length.Should().Be(2);

        A.CallTo(fakeWriter)
         .WithReturnType<Task>()
         .Where(x => x.Method.Name == nameof(fakeWriter.Write))
         .WithAnyArguments()
         .Invokes(x => callArguments = x.Arguments);

        var repository = new TestAggregateRepository(A.Fake<IAggregateStreamReader>(), fakeWriter);
        await repository.Save(aggregate, default);

        callArguments.Should().NotBeNull();
        callArguments!.Count.Should().Be(4);
        var capturedStreamId = (string)callArguments[0]!;
        var capturedVersion = (StreamRevision)callArguments[1]!;
        var capturedEvents = (IEnumerable<IAggregateEvent>)callArguments[2]!;
        callArguments[3]!.Should().BeOfType<CancellationToken>();

        capturedStreamId.Should().Be(repository.StreamPrefix + aggregateId);
        capturedVersion.Should().Be(aggregate.OriginalVersion.ToStreamRevision());

        for (var i = 0; i < pendingEvents.Length; i++)
        {
            var pendingEvent = pendingEvents[i];
            var capturedEvent = capturedEvents.ElementAt(i);
            capturedEvent.Should().Be(pendingEvent);
        }
    }
}
