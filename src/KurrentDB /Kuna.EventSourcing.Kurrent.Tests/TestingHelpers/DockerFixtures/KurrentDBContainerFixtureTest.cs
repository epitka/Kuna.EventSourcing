using DotNet.Testcontainers.Containers;

namespace Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.DockerFixtures;

public class KurrentDBContainerFixtureTest
{
    //// [Fact(Skip = "Just to test bootstrapping container works correctly.")]
    [Fact]
    public async Task Can_Start_EventStore_Docker_Container()
    {
        // To see that container is running
        // uncomment WithCleanUp and WithAutoRemove and navigate to http://localhost:2113 to see EventStore dashboard
        var esContainer = KurrentDBContainerFixture.KurrentDBContainerBuilder
                                                    .WithCleanUp(true)
                                                    .WithAutoRemove(true)
                                                    .Build();

        await esContainer.StartAsync(TestContext.Current.CancellationToken);

        esContainer.State.Should().Be(TestcontainersStates.Running);
    }
}
