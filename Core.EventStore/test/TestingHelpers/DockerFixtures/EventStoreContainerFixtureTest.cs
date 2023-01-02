using DotNet.Testcontainers.Containers;

namespace Kuna.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;

public class EventStoreContainerFixtureTest
{
    [Fact(Skip = "Just to test bootstrapping container works correctly.")]
    public async Task Can_Start_EventStore_Docker_Container()
    {
        // To see that container is running
        // uncomment WithCleanUp and WithAutoRemove and navigate to http://localhost:2113 to see EventStore dashboard
        var esContainer = EventStoreContainerFixture.EventStoreContainer()
                                                    // .WithCleanUp(false)
                                                    // .WithAutoRemove(false)
                                                    .Build();

        await esContainer.StartAsync();

        esContainer.State.Should().Be(TestcontainersStates.Running);
    }
}
