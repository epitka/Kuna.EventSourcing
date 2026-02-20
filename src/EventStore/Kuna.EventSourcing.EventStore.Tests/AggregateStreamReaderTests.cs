using Kuna.EventSourcing.EventStore.Configuration;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers.DockerFixtures;
using Kuna.EventSourcing.EventStore.Tests.TestingHelpers.XUnitHelpers;
using Microsoft.Extensions.Configuration;
using static System.String;
using static Kuna.EventSourcing.EventStore.Tests.TestingHelpers.HelperFunctions;

namespace Kuna.EventSourcing.EventStore.Tests;

[Collection("EventStore collection")]
public class AggregateStreamReaderTests
{
    private static readonly string StreamPrefix = "readTest-";
    private static readonly Guid AggregateId = Guid.NewGuid();


    public AggregateStreamReaderTests(EventStoreContainerFixture eventStoreDatabaseFixture)
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
    public async Task Can_Read_Events()
    {
        using var scope = this.ServiceProvider.CreateScope();

        var writer = scope.ServiceProvider.GetRequiredService<IStreamWriter>();
        var reader = scope.ServiceProvider.GetRequiredService<IStreamReader>();

        var streamId = Concat(StreamPrefix, AggregateId);
        var events = GetEvents(AggregateId, 10);

        await writer.Write(streamId, (-1).ToStreamRevision(), events, CancellationToken.None);

        var fetchedEvents = (await reader.GetEvents(streamId, CancellationToken.None)).ToArray();

        fetchedEvents.Should().NotBeNull();
        fetchedEvents.Length.Should().Be(events.Length);

        for (var i = 0; i < fetchedEvents.Length; i++)
        {
            var @event = fetchedEvents[i];
            @event.Should().BeOfType<TestAggregateEvent>();

            @event.Should().Be(events[i]);
        }
    }

    [Fact]
    public async Task When_Aggregate_Stream_Does_Not_Exist_Should_Return_Empty_Enumerable()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var reader = scope.ServiceProvider.GetRequiredService<IStreamReader>();

        var streamId = Concat(StreamPrefix, Guid.NewGuid());

        var result = await reader.GetEvents(streamId, CancellationToken.None);

        result.Any().Should().Be(false);
    }
}
