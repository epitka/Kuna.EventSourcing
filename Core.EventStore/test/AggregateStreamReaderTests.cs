using System.Text;
using EventStore.Client;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.XUnitHelpers;
using Senf.EventSourcing.Core.Exceptions;
using static System.String;
using static Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.HelperFunctions;

namespace Senf.EventSourcing.Core.EventStore.Tests;

[Collection("EventStore collection")]
public class AggregateStreamReaderTests
{
    private readonly EventStoreContainerFixture eventStoreDatabaseFixture;

    private static readonly string streamPrefix = "readTest-";
    private static readonly Guid aggregateId = Guid.NewGuid();

    public AggregateStreamReaderTests(EventStoreContainerFixture eventStoreDatabaseFixture)
    {
        this.eventStoreDatabaseFixture = eventStoreDatabaseFixture;
        this.ServiceProvider = this.eventStoreDatabaseFixture.ServiceProvider;
    }

    private IServiceProvider ServiceProvider { get; }

    [Fact]
    [TestPriority(0)]
    public async Task Can_Read_Events()
    {
        using var scope = this.eventStoreDatabaseFixture.ServiceProvider.CreateScope();

        var writer = scope.ServiceProvider.GetRequiredService<IAggregateStreamWriter>();
        var reader = scope.ServiceProvider.GetRequiredService<IAggregateStreamReader>();

        var streamId = Concat(streamPrefix, aggregateId);
        var events = GetEvents(aggregateId, 10);

        await writer.Write(streamId, (-1).ToStreamRevision(), events, default);

        var fetchedEvents = (await reader.GetEvents(streamId, default)).ToArray();

        fetchedEvents.Should().NotBeNull();
        fetchedEvents.Count().Should().Be(events.Length);

        for (var i = 0; i < fetchedEvents.Count(); i++)
        {
            var @event = fetchedEvents[i];
            @event.Should().BeOfType<TestEvent>();

            @event.Should().Be(events[i]);
        }
    }

    [Fact]
    public async Task When_Aggregate_Stream_Does_Not_Exist_Should_Return_Empty_Enumerable()
    {
        using var scope = this.eventStoreDatabaseFixture.ServiceProvider.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IAggregateStreamReader>();

        var streamId = Concat(streamPrefix, Guid.NewGuid());

        var result = await reader.GetEvents(streamId, default);

        result.Any().Should().Be(false);
    }
}
