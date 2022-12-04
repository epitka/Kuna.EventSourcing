namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers.DockerFixtures;

[CollectionDefinition("EventStore collection")]
public class DatabaseCollection : ICollectionFixture<EventStoreContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
