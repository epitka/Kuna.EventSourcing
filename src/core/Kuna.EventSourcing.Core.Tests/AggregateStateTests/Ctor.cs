namespace Kuna.EventSourcing.Core.Tests.AggregateStateTests;

public class Ctor
{
    [Fact]
    public void Should_Construct_With_Correct_Defaults()
    {
        var sut = new TestAggregate.State();
        sut.Version.Should().Be(-1);
        sut.Id.Should().Be(null);
    }
}
