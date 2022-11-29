namespace Senf.EventSourcing.Core.Tests.AggregateTests;

public class Ctor
{
    [Fact]
    public void Should_Construct_With_Correct_Default_Values()
    {
        var sut = new TestAggregate();
        sut.Version.Should().Be(-1);
        sut.Id.Should().Be(Guid.Empty);
        sut.GetPendingEvents().Count().Should().Be(0);
    }
}
