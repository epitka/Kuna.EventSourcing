namespace Kuna.EventSourcing.Kurrent.Tests.TestingHelpers.DockerFixtures;

[CollectionDefinition("KurrentDB collection")]
public class DatabaseCollection : ICollectionFixture<KurrentDBContainerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
