using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Kuna.EventSourcing.EventStore.Tests.TestingHelpers.DockerFixtures;

public class EventStoreContainerFixture
    : IAsyncLifetime
{
    public EventStoreContainerFixture()
    {


        this.EventStoreDockerContainer = EventStoreContainer()
                                         .WithAutoRemove(true)
                                         .WithCleanUp(true)
                                         .Build();
    }
    public IContainer EventStoreDockerContainer { get; }


    public async Task InitializeAsync()
    {
        await this.EventStoreDockerContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await this.EventStoreDockerContainer.StopAsync();
        await this.EventStoreDockerContainer.DisposeAsync();
    }

    /// <summary>
    /// Configures EventStore container with latest version of EventStore, that will have docker container and resources automatically
    /// removed.
    /// </summary>
    public static ContainerBuilder EventStoreContainer()
    {
        var builder = new ContainerBuilder()
                      .WithImage("eventstore/eventstore:latest")
                      .WithName("eventstore-test-" + Guid.NewGuid().ToString("N"))
                      .WithEnvironment("EVENTSTORE_CLUSTER_SIZE", "1")
                      .WithEnvironment("EVENTSTORE_RUN_PROJECTIONS", "All")
                      .WithEnvironment("EVENTSTORE_START_STANDARD_PROJECTIONS", "true")
                      .WithEnvironment("EVENTSTORE_EXT_TCP_PORT", "1117")
                      .WithEnvironment("EVENTSTORE_HTTP_PORT", "2117")
                      .WithEnvironment("EVENTSTORE_INSECURE", "true")
                      .WithEnvironment("EVENTSTORE_ENABLE_EXTERNAL_TCP", "true")
                      .WithEnvironment("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP", "true")
                      .WithEnvironment("EVENTSTORE_ADVERTISE_HOST_TO_CLIENT_AS", "127.0.0.1")
                      .WithPortBinding("1117", "1117")
                      .WithPortBinding("2117", "2117")
                      .WithExposedPort(1117)
                      .WithExposedPort(2117)
                      .WithHostname(Dns.GetHostName())
                      .WithWaitStrategy(Wait.ForUnixContainer().UntilExternalTcpPortIsAvailable(1117))
                      .WithAutoRemove(true)
                      .WithCleanUp(true);

        return builder;
    }
}
