using System.Text;
using EventStore.Client;
using Kuna.EventSourcing.Core.Exceptions;
using Kuna.EventSourcing.EventStore.Configuration;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers.DockerFixtures;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers.XUnitHelpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static Kuna.EventSourcing.EventStore.Tests.TestingHelpers.HelperFunctions;

namespace Kuna.EventSourcing.EventStore.Tests;

[Collection("EventStore collection")]
[TestCaseOrderer("Kuna.EventSourcing.EventStore.Tests.TestingHelpers.XUnitHelpers.PriorityOrderer", "Kuna.EventSourcing.EventStore.Tests")]
public class AggregateStreamWriterTests
{
    private static readonly string StreamPrefix = "writeTest-";
    private static readonly Guid AggregateId = Guid.NewGuid();

    public AggregateStreamWriterTests(EventStoreContainerFixture eventStoreDatabaseFixture)
    {
        var cfg =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false);

        var sc = new ServiceCollection();

        sc.AddLogging();
        sc.AddEventStore(
            cfg.Build(),
            "EventStore",
            new[] { this.GetType().Assembly },
            assemblies =>  new []{typeof(TestAggregateEvent)});

        this.ServiceProvider = sc.BuildServiceProvider();
    }

    private IServiceProvider ServiceProvider { get; }

    [Fact]
    [TestPriority(0)]
    public async Task Can_Write_Events_To_New_Stream()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IStreamWriter>();

        var streamId = GetStreamId(StreamPrefix, AggregateId);
        var events = GetEvents(AggregateId, 10);

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
    public async Task Can_Write_Events_To_Existing_Stream()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IStreamWriter>();
        var client = this.ServiceProvider.GetRequiredService<EventStoreClient>();

        var events = GetEvents(AggregateId, 10);
        var streamId = GetStreamId(StreamPrefix, AggregateId);

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
    public async Task When_Invalid_Expected_Version_Is_Supplied_Throws_InvalidExpectedVersionException()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IStreamWriter>();
        var client = this.ServiceProvider.GetRequiredService<EventStoreClient>();

        var events = GetEvents(AggregateId, 1);
        var streamId = GetStreamId(StreamPrefix, AggregateId);

        // let's first fetch events from the stream so we can get the position of last event
        var expectedVersion = await GetExpectedVersion(client, streamId);

        await Assert.ThrowsAsync<AggregateInvalidExpectedVersionException>(
            async () => await writer.Write(streamId, (expectedVersion + 1).ToStreamRevision(), events, default));

        await Assert.ThrowsAsync<AggregateInvalidExpectedVersionException>(
            async () => await writer.Write(streamId, (expectedVersion - 1).ToStreamRevision(), events, default));
    }
}
