using System.Diagnostics;
using System.Text;
using AutoFixture;
using EventStore.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.XUnitHelpers;
using static Senf.EventSourcing.Core.EventStore.Extensions;

namespace Senf.EventSourcing.Core.EventStore.Tests;

public class AggregateStreamWriterTests
    : IClassFixture<EventStoreContainerFixture>
{

    private readonly EventStoreContainerFixture eventStoreDatabaseFixture;

    public AggregateStreamWriterTests(EventStoreContainerFixture eventStoreDatabaseFixture)
    {
        this.eventStoreDatabaseFixture = eventStoreDatabaseFixture;
        this.ServiceProvider = this.eventStoreDatabaseFixture.ServiceProvider;
    }

    private IServiceProvider ServiceProvider { get;}

    [Fact]
    //[TestPriority(0)]
    public async Task Can_Write_Events_To_New_Stream()
    {
        using var scope = this.eventStoreDatabaseFixture.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IAggregateStreamWriter>();

        var aggregateId = Guid.NewGuid();
        var streamId = "writeTest-" + aggregateId.ToString().Replace("-", "").ToLower();
        var events = this.GetEvents(aggregateId, 0, 10);

        await writer.Write(streamId, (-1).ToStreamRevision(), events, default);

        // verify they have been written
        var client = this.ServiceProvider.GetRequiredService<EventStoreClient>();

        var result = client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start);

        var fetchedEvents = await result.Select(x => x).ToArrayAsync();

         fetchedEvents.Length.Should().Be(events.Length);

        var serializer = this.ServiceProvider.GetRequiredService<IEventSerializer>();

        foreach (var resolvedEvent in fetchedEvents)
        {
            var @event =  serializer.Deserialize(resolvedEvent);
            @event.Should().BeAssignableTo<TestEvent>();

            var metaData = JsonConvert.DeserializeObject(
                Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span),
                typeof(Dictionary<string, string>),
                JsonEventSerializer.SerializerSettings);

            metaData.Should().NotBeNull();
        }
    }

    /*[Fact]
    [TestPriority(3)]
    public async void Can_Write_Events_To_Existing_Stream()
    {
        using var scope = this.ServiceProvider.CreateScope();

        var connection = scope.ServiceProvider.GetRequiredService<IEventStoreConnection>();

        connection.ShouldNotBeNull("Cannot open connection to event store");

        var writer = scope.ServiceProvider.GetRequiredService<IEventStoreStreamWriter>();

        var events = this.GetEvents(0, 10);

        var originalEvents = await connection.ReadStreamEventsForwardAsync(StreamId, 0, 500, false);

        var lastPosition = originalEvents.LastEventNumber;

        await writer.WriteEvents(
            StreamId,
            lastPosition,
            events,
            Headers);

        // verify they have been written
        var currentEvents = await connection.ReadStreamEventsForwardAsync(StreamId, 0, 500, false);

        currentEvents.Events.Length.ShouldBe(events.Count() + originalEvents.Events.Count());
    }

    [Fact]
    [TestPriority(4)]
    public async void Throws_When_Invalid_Expected_Version_Is_Supplied()
    {
        using var scope = this.ServiceProvider.CreateScope();

        var connection = scope.ServiceProvider.GetRequiredService<IEventStoreConnection>();

        connection.ShouldNotBeNull("Cannot open connection to event store");

        var writer = scope.ServiceProvider.GetRequiredService<IEventStoreStreamWriter>();

        var events = this.GetEvents(0, 10);

        var originalEvents = await connection.ReadStreamEventsForwardAsync(StreamId, 0, 500, false);

        var lastPosition = originalEvents.LastEventNumber;

        await Assert.ThrowsAsync<InvalidExpectedVersionException>(
            async () => await writer.WriteEvents(
                StreamId,
                lastPosition + 1,
                events,
                Headers));

        await Assert.ThrowsAsync<InvalidExpectedVersionException>(
            async () => await writer.WriteEvents(
                StreamId,
                lastPosition - 1,
                events,
                Headers));
    }*/

    private TestEvent[] GetEvents(Guid aggregateId, int startIdx, int count)
    {
        var toReturn = Enumerable.Range(startIdx, count)
                                 .Select(
                                     x =>
                                     {
                                         var @event = new TestEvent(aggregateId, x.ToString());
                                         return @event;
                                     })
                                 .ToArray();

        return toReturn;
    }
}
