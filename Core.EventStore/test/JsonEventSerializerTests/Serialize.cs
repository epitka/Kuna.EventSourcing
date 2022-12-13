using System.Text;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Tests.JsonEventSerializerTests;

public class Serialize
{
    private sealed record Serialized(Guid Id, string Name) : IAggregateEvent
    {
    }

    [Fact]
    public void Should_Serialize_To_Byte_Array()
    {
        var @event = new Serialized(Guid.NewGuid(), "test");

        var fakeEventTypeMapper = A.Fake<IEventTypeMapper>(opt => opt.Strict());

        var serializer = new JsonEventSerializer(fakeEventTypeMapper);

        var serialized = serializer.Serialize(@event);

        var deserialized = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(serialized),
            typeof(Serialized),
            JsonEventSerializer.SerializerSettings);

        deserialized.Should().Be(@event);
    }
}
