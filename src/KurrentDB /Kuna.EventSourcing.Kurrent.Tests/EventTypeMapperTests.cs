using Kuna.EventSourcing.Kurrent.Tests.TestingHelpers;

namespace Kuna.EventSourcing.Kurrent.Tests;

public class EventTypeMapperTests
{
    private readonly EventTypeMapper mapper;

    public EventTypeMapperTests()
    {
        this.mapper= new EventTypeMapper(
            new[] { this.GetType().Assembly },
            assemblies =>  new []{typeof(TestAggregateEvent)}
        );
    }

    [Fact]
    public void When_Entry_For_Type_Name_Does_Not_Exist_Should_Throw()
    {
        var testEventName = Guid.NewGuid().ToString();

        Assert.Throws<InvalidOperationException>(()=>this.mapper.MapFrom(testEventName));

    }

    [Fact]
    public void Should_Return_Type()
    {
        var testEventName = nameof(TestAggregateEvent);

        var eventType = this.mapper.MapFrom(testEventName);

        eventType.Should().NotBeNull();

        eventType.Should().Be(typeof(TestAggregateEvent));
    }
}
