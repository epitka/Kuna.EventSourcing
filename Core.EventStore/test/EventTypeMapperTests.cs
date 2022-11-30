using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Tests;

public class EventTypeMapperTests
{
    private readonly EventTypeMapper mapper;

    public class DummyEvent : IEvent
    {

    }

    public EventTypeMapperTests()
    {
        this.mapper= new EventTypeMapper(new[] { this.GetType().Assembly });
    }

    [Fact]
    public void When_Entry_For_Type_Name_Does_Not_Exist_Should_Return_Null()
    {
        var testEventName = Guid.NewGuid().ToString();

        var eventType = this.mapper.MapFrom(testEventName);

        eventType.Should().BeNull();
    }

    [Fact]
    public void Should_Return_Type()
    {
        var testEventName = nameof(DummyEvent);

        var eventType = this.mapper.MapFrom(testEventName);

        eventType.Should().NotBeNull();

        eventType.Should().Be(typeof(DummyEvent));
    }
}
