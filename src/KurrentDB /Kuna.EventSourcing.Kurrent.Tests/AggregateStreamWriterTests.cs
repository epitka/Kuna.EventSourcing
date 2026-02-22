using System.Text;
using Kuna.EventSourcing.Core.Exceptions;
using Kuna.EventSourcing.Kurrent.Configuration;
using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers;
using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.DockerFixtures;
using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.XUnitHelpers;
using KurrentDB.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using static Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.HelperFunctions;

namespace Kuna.EventSourcing.Kurrent.Tests;

[Collection("KurrentDB collection")]
[TestCaseOrderer(typeof(PriorityOrderer))]
public class AggregateStreamWriterTests
{
    private static readonly string StreamPrefix = "writeTest-";
    private static readonly Guid AggregateId = Guid.NewGuid();

    public AggregateStreamWriterTests(KurrentDBContainerFixture kurrentDBDatabaseFixture)
    {
        var port = kurrentDBDatabaseFixture.KurrentDBTestContainer.GetMappedPublicPort();

        var cfg =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        {"ConnectionStrings:KurrentDB", $"kurrentdb://admin:changeit@localhost:{port}?tls=false&tlsVerifyCert=false"},
                    });

        var sc = new ServiceCollection();

        sc.AddLogging();
        sc.AddKurrentDB(
            cfg.Build(),
            "KurrentDB",
            new[] { this.GetType().Assembly },
            assemblies => new[] { typeof(TestAggregateEvent) });

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

        await writer.Write(streamId, null, events, CancellationToken.None);

        // verify they have been written
        var client = this.ServiceProvider.GetRequiredService<KurrentDBClient>();

        var result = client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start, cancellationToken: TestContext.Current.CancellationToken);

        var fetchedEvents = new List<ResolvedEvent>();

        await foreach (var ev in result)
        {
            fetchedEvents.Add(ev);
        }

        fetchedEvents.Count.Should().Be(events.Length);

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
        var client = this.ServiceProvider.GetRequiredService<KurrentDBClient>();

        var events = GetEvents(AggregateId, 10);
        var streamId = GetStreamId(StreamPrefix, AggregateId);

        // let's first fetch events from the stream so we can get the position of last event
        var expectedVersion = await GetExpectedVersion(client, streamId);

        // now lets write new events
        await writer.Write(streamId, expectedVersion, events, CancellationToken.None);

        // verify they have been written
        var result = client.ReadStreamAsync(Direction.Forwards, streamId, StreamPosition.Start, cancellationToken: TestContext.Current.CancellationToken);

        var fetchedEvents = new List<ResolvedEvent>();

        await foreach (var ev in result)
        {
            fetchedEvents.Add(ev);
        }

        fetchedEvents.Count.Should().Be(events.Length + (int)(expectedVersion + 1));
    }

    [Fact]
    [TestPriority(2)]
    public async Task When_Invalid_Expected_Version_Is_Supplied_Throws_InvalidExpectedVersionException()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IStreamWriter>();
        var client = this.ServiceProvider.GetRequiredService<KurrentDBClient>();

        var events = GetEvents(AggregateId, 1);
        var streamId = GetStreamId(StreamPrefix, AggregateId);

        // let's first fetch events from the stream so we can get the position of last event
        var expectedVersion = await GetExpectedVersion(client, streamId);

        await Assert.ThrowsAsync<AggregateInvalidExpectedVersionException>(async () => await writer.Write(streamId, expectedVersion + 1, events, CancellationToken.None));

        await Assert.ThrowsAsync<AggregateInvalidExpectedVersionException>(async () => await writer.Write(streamId, expectedVersion - 1, events, CancellationToken.None));
    }
}
