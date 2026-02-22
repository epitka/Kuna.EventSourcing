using System.Text;
using Newtonsoft.Json;

namespace Kuna.EventSourcing.Kurrent.Tests;

public class EventDataFactoryTests
{
    private record DummyAggregateEvent(Guid Id, string Name)
    {
    }

    [Fact]
    public void From_Should_Return_Instance_Of_EventData()
    {
        var @event = new DummyAggregateEvent(Guid.NewGuid(), "test");

        var fakeEventTypeMapper = A.Fake<IEventTypeMapper>(opt => opt.Strict());

        A.CallTo(() => fakeEventTypeMapper.MapFrom(nameof(DummyAggregateEvent)))
         .Returns(typeof(DummyAggregateEvent));

        var fakeMetaDataFactory = A.Fake<IEventMetadataFactory>(opt => opt.Strict());
        A.CallTo(() => fakeMetaDataFactory.Get())
         .Returns(new Dictionary<string, string>() { { "test", "test" } });

        var serializer = new JsonEventStoreSerializer(fakeEventTypeMapper);
        var eventDataFactory = new EventDataFactory(serializer, fakeMetaDataFactory);

        var result = eventDataFactory.From(@event);

        result.Type.Should().Be(nameof(DummyAggregateEvent));

        var deserializedEvent = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(result.Data.Span),
            typeof(DummyAggregateEvent),
            JsonEventStoreSerializer.SerializerSettings);

        deserializedEvent.Should().Be(@event);

        var deserializedMetadata = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(result.Metadata.Span),
            typeof(Dictionary<string,string>),
            JsonEventStoreSerializer.SerializerSettings) as Dictionary<string,string>;

        deserializedMetadata.Should().NotBeNull();
        deserializedMetadata!.ContainsKey("test").Should().Be(true);
    }
}
