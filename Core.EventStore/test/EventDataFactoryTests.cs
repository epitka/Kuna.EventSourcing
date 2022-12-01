using System.Text;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Senf.EventSourcing.Core.EventStore.Tests;

public class EventDataFactoryTests
{
    private record DummyEvent(Guid Id, string Name) : IEvent
    {
    }

    [Fact]
    public void From_Should_Return_Instance_Of_EventData()
    {
        var @event = new DummyEvent(Guid.NewGuid(), "test");

        var fakeEventTypeMapper = A.Fake<IEventTypeMapper>(opt => opt.Strict());

        A.CallTo(() => fakeEventTypeMapper.MapFrom(nameof(DummyEvent)))
         .Returns(typeof(DummyEvent));

        var fakeMetaDataFactory = A.Fake<IEventMetadataFactory>(opt => opt.Strict());
        A.CallTo(() => fakeMetaDataFactory.Get())
         .Returns(new Dictionary<string, string>() { { "test", "test" } });

        var serializer = new JsonEventSerializer(fakeEventTypeMapper);
        var eventDataFactory = new EventDataFactory(serializer, fakeMetaDataFactory);

        var result = eventDataFactory.From(@event);

        result.Type.Should().Be(nameof(DummyEvent));

        var deserializedEvent = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(result.Data.Span),
            typeof(DummyEvent),
            JsonEventSerializer.SerializerSettings);

        deserializedEvent.Should().Be(@event);

        var deserializedMetadata = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(result.Metadata.Span),
            typeof(Dictionary<string,string>),
            JsonEventSerializer.SerializerSettings) as Dictionary<string,string>;

        deserializedMetadata.Should().NotBeNull();
        deserializedMetadata!.ContainsKey("test").Should().Be(true);
    }
}
