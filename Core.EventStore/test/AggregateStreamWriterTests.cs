using System.Text;
using EventStore.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.XUnitHelpers;
using Senf.EventSourcing.Core.Exceptions;
using static Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.HelperFunctions;

namespace Senf.EventSourcing.Core.EventStore.Tests;

[Collection("EventStore collection")]
[TestCaseOrderer("Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.XUnitHelpers.PriorityOrderer", "Senf.EventSourcing.Core.EventStore.Tests")]
public class AggregateStreamWriterTests
{
    private readonly EventStoreContainerFixture eventStoreDatabaseFixture;

    private static readonly string streamPrefix = "writeTest-";
    private static readonly Guid aggregateId = Guid.NewGuid();

    public AggregateStreamWriterTests(EventStoreContainerFixture eventStoreDatabaseFixture)
    {
        this.eventStoreDatabaseFixture = eventStoreDatabaseFixture;
        this.ServiceProvider = this.eventStoreDatabaseFixture.ServiceProvider;
    }

    private IServiceProvider ServiceProvider { get; }

    [Fact]
    [TestPriority(0)]
    public async Task Can_Write_Events_To_New_Stream()
    {
        using var scope = this.eventStoreDatabaseFixture.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IAggregateStreamWriter>();

        var streamId = GetStreamId(streamPrefix, aggregateId);
        var events = GetEvents(aggregateId, 10);

        await writer.Write(streamId, (-1).ToStreamRevision(), events, default);

        // verify they have been written
        var client = this.ServiceProvider.GetRequiredService<EventStoreClient>();

        var result = client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start);

        var fetchedEvents = await result.Select(x => x).ToArrayAsync();

        fetchedEvents.Length.Should().Be(events.Length);

        var serializer = this.ServiceProvider.GetRequiredService<IEventStoreSerializer>();

        foreach (var resolvedEvent in fetchedEvents)
        {
            var @event = serializer.DeserializeData(resolvedEvent);
            @event.Should().BeAssignableTo<TestAggregateEvent>();

            var metaData = JsonConvert.DeserializeObject(
                Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span),
                typeof(Dictionary<string, string>),
                JsonEventStoreSerializer.SerializerSettings);

            metaData.Should().NotBeNull();
        }
    }

    [Fact]
    [TestPriority(1)]
    public async void Can_Write_Events_To_Existing_Stream()
    {
        using var scope = this.eventStoreDatabaseFixture.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IAggregateStreamWriter>();
        var client = this.ServiceProvider.GetRequiredService<EventStoreClient>();

        var events = GetEvents(aggregateId, 10);
        var streamId = GetStreamId(streamPrefix, aggregateId);

        // let's first fetch events from the stream so we can get the position of last event
       var expectedVersion = await GetExpectedVersion(client, streamId);

        // now lets write new events
        await writer.Write(streamId, expectedVersion.ToStreamRevision(), events, default);

        // verify they have been written
        var result = client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start);

        var fetchedEvents = await result.Select(x => x).ToArrayAsync();

        fetchedEvents.Length.Should().Be(events.Length + (expectedVersion + 1));
    }

    [Fact]
    [TestPriority(2)]
    public async void When_Invalid_Expected_Version_Is_Supplied_Throws_InvalidExpectedVersionException()
    {
        using var scope = this.eventStoreDatabaseFixture.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IAggregateStreamWriter>();
        var client = this.ServiceProvider.GetRequiredService<EventStoreClient>();

        var events = GetEvents(aggregateId, 1);
        var streamId = GetStreamId(streamPrefix, aggregateId);

        // let's first fetch events from the stream so we can get the position of last event
        var expectedVersion = await GetExpectedVersion(client, streamId);

        await Assert.ThrowsAsync<InvalidExpectedVersionException>(
            async () => await writer.Write(streamId, (expectedVersion + 1).ToStreamRevision(), events, default));

        await Assert.ThrowsAsync<InvalidExpectedVersionException>(
            async () => await writer.Write(streamId, (expectedVersion - 1).ToStreamRevision(), events, default));
    }
}
