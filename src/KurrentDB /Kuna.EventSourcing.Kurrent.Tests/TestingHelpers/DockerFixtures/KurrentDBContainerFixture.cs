using DotNet.Testcontainers.Containers;
using Testcontainers.KurrentDb;

namespace Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.DockerFixtures;

public class KurrentDBContainerFixture
    : IAsyncLifetime
{
    public KurrentDBContainerFixture()
    {

        this.KurrentDBTestContainer = new KurrentDbBuilder("kurrentplatform/kurrentdb:25.1")
                                         .WithAutoRemove(true)
                                         .WithCleanUp(true)
                                         .Build();
    }
    public IContainer KurrentDBTestContainer { get; }


    public async ValueTask InitializeAsync()
    {
        await this.KurrentDBTestContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await this.KurrentDBTestContainer.StopAsync();
        await this.KurrentDBTestContainer.DisposeAsync();
    }

    public static KurrentDbBuilder KurrentDBContainerBuilder => new KurrentDbBuilder("kurrentplatform/kurrentdb:25.1")
                                                                .WithPortBinding("2117","2117")
                                                                .WithExposedPort(2117);


    /*/// <summary>
    /// Configures EventStore container with latest version of EventStore, that will have docker container and resources automatically
    /// removed.
    /// </summary>
    public static ContainerBuilder KurrentDBContainer()
    {

        var builder = new ContainerBuilder()
                      .WithImage("docker.kurrent.io/kurrent-latest/kurrentdb:latest")
                      .WithName("kurrentdb-test-" + Guid.NewGuid().ToString("N"))
                      .WithEnvironment("KURRENTDB_CLUSTER_SIZE", "1")
                      .WithEnvironment("KURRENTDB_RUN_PROJECTIONS", "All")
                      .WithEnvironment("KURRENTDB_START_STANDARD_PROJECTIONS", "true")
                      .WithEnvironment("KURRENTDB_NODE_PORT", "2117")
                      .WithEnvironment("KURRENTDB_INSECURE", "true")
                      .WithEnvironment("KURRENTDB_ENABLE_ATOM_PUB_OVER_HTTP", "true")
                      .WithEnvironment("KURRENTDB_ADVERTISE_HOST_TO_CLIENT_AS", "127.0.0.1")
                      .WithPortBinding("2117", "2117");
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
    }*/
}
