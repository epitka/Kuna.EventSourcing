using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;

public class EventStoreContainerFixture
    : IAsyncLifetime
{
    private static string EventStoreConnectionString { get; set; } = "esdb+discover://localhost:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000";

    public EventStoreContainerFixture()
    {
        this.EventStoreDockerContainer = EventStoreContainer()
                                         .WithAutoRemove(false)
                                         .WithCleanUp(false)
                                         .Build();

        var services = BootstrapServiceCollection();
        this.ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; }

    public TestcontainersContainer EventStoreDockerContainer { get; }

    public async Task InitializeAsync()
    {
        await this.EventStoreDockerContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await this.EventStoreDockerContainer.CleanUpAsync();
        await this.EventStoreDockerContainer.DisposeAsync();
    }


    /// <summary>
    /// Configures EventStore container with latest version of EventStore, that will have docker container and resources automatically
    /// removed.
    /// </summary>
    public static ITestcontainersBuilder<TestcontainersContainer> EventStoreContainer()
    {
        var builder = new TestcontainersBuilder<TestcontainersContainer>()
                      .WithImage("eventstore/eventstore:latest")
                      .WithName("eventstore-test-" + Guid.NewGuid().ToString("N"))
                      .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
                      .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
                      .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
                      .WithEnvironment("EVENTSTORE_EXT_TCP_PORT", "1113")
                      .WithEnvironment("EVENTSTORE_HTTP_PORT", "2113")
                      .WithEnvironment("EVENTSTORE_INSECURE", "true")
                      .WithEnvironment("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true")
                      .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
                      .WithEnvironment("EVENTSTORE_ADVERTISE_HOST_TO_CLIENT_AS", "127.0.0.1")
                      .WithPortBinding("1113", "1113")
                      .WithPortBinding("2113", "2113")
                      .WithExposedPort(1113)
                      .WithExposedPort(2113)
                      .WithHostname(Dns.GetHostName())
                      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1113))
                      .WithAutoRemove(true)
                      .WithCleanUp(true);

        return builder;
    }

    private static ServiceCollection BootstrapServiceCollection()
    {
        var sc = new ServiceCollection();

        sc.AddSingleton<IEventTypeMapper, EventTypeMapper>()
          .AddSingleton<IEventSerializer, JsonEventSerializer>()
          .AddSingleton<IEventMetadataFactory, EventMetadataFactory>()
          .AddSingleton<IEventDataFactory, EventDataFactory>()
          .AddSingleton<IAggregateStreamWriter, AggregateStreamWriter>()
          .AddSingleton<IAggregateStreamReader, AggregateStreamReader>();

        sc.AddLogging();

        sc.AddSingleton<EventStoreClient>(
            sp =>
            {
                var settings = EventStoreClientSettings
                    .Create(EventStoreConnectionString);

                settings.DefaultCredentials = new UserCredentials("admin", "changeit");

                settings.ConnectionName = "test-" + Guid.NewGuid().ToString();

                return new EventStoreClient(settings);
            });

        return sc;
    }
}
