namespace Kuna.EventSourcing.Core.Tests.AggregateTests;

public class Ctor
{
    [Fact]
    public void Should_Construct_With_Correct_Default_Values()
    {
        var sut = new TestAggregate();
        sut.Version.Should().Be(null);
        sut.OriginalVersion.Should().Be(null);
        sut.Id.Should().Be(null);
        sut.GetPendingEvents().Length.Should().Be(0);
    }
}
