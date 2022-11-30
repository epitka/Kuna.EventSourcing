using System.Diagnostics;
using EventStore.Client;
using FakeItEasy;
using FluentAssertions;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Tests.JsonEventSerializerTests;

public class Deserialize
{
    private sealed record Deserialized(Guid Id, string Name) : IEvent
    {
    }

    [Fact]
    public void When_Missing_EventType_Definition_Should_Throw()
    {
        // if we have event in the event store but we do not have
        // definition of event in our code, when trying to deserialize
        // it should throw exception.
        var @event = new Deserialized(Guid.NewGuid(), "test");

        const string eventStreamId = "myStreamid-1";
        const int eventNumber = 0;
        var eventId = Uuid.NewUuid();
        var position = new Position(0, 0);

        var fakeEventTypeMapper = A.Fake<IEventTypeMapper>(opt => opt.Strict());
        A.CallTo(() => fakeEventTypeMapper.MapFrom(A<string>._))
         .Returns(null);

        var serializer = new JsonEventSerializer(fakeEventTypeMapper);
        var data = serializer.Serialize(@event);

        // here we are pretending that this came from EventStore
        // when appending to stream, it will automatically set meta-data provided manually here
        var eventRecord = new EventRecord(
            eventStreamId,
            eventId,
            eventNumber,
            position,
            new Dictionary<string, string>()
            {
                { "type", "UnknownEventType" },
                {"created", 100.ToString()},
                {"content-type", "application/json"},
            },
            data,
            null);

        var resolvedEvent = new ResolvedEvent(eventRecord, null, null);

        Assert.Throws<InvalidOperationException>(()=> serializer.Deserialize(resolvedEvent));
    }

    [Fact]
    public void Should_Return_Instance_Of_Event()
    {
        var @event = new Deserialized(Guid.NewGuid(), "test");

        const string eventStreamId = "myStreamid-1";
        const int eventNumber = 0;
        var eventId = Uuid.NewUuid();
        var position = new Position(0, 0);

        var fakeEventTypeMapper = A.Fake<IEventTypeMapper>(opt => opt.Strict());
        A.CallTo(() => fakeEventTypeMapper.MapFrom(nameof(Deserialized)))
         .Returns(typeof(Deserialized));

        var serializer = new JsonEventSerializer(fakeEventTypeMapper);
        var data = serializer.Serialize(@event);

        // here we are pretending that this came from EventStore
        // when appending to stream, it will automatically set meta-data provided manually here
        var eventRecord = new EventRecord(
            eventStreamId,
            eventId,
            eventNumber,
            position,
            new Dictionary<string, string>()
            {
                { "type", nameof(Deserialized) },
                {"created", 100.ToString()},
                {"content-type", "application/json"},
            },
            data,
            null);

        var resolvedEvent = new ResolvedEvent(eventRecord, null, null);

        var deserialized = serializer.Deserialize(resolvedEvent) as Deserialized;

        deserialized.Should().NotBeNull();

        deserialized.Should().Be(@event);
        deserialized!.Id.Should().Be(@event.Id);
        deserialized!.Name.Should().Be(@event.Name);
    }
}
