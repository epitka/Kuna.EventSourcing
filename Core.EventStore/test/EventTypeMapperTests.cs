using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

namespace Senf.EventSourcing.Core.EventStore.Tests;

public class EventTypeMapperTests
{
    private readonly EventTypeMapper mapper;

    public EventTypeMapperTests()
    {
        this.mapper= new EventTypeMapper(new[] { this.GetType().Assembly });
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
