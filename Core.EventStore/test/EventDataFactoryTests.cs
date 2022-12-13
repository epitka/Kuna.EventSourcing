﻿using System.Text;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Senf.EventSourcing.Core.EventStore.Tests;

public class EventDataFactoryTests
{
    private record DummyAggregateEvent(Guid Id, string Name) : IAggregateEvent
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

        var serializer = new JsonEventSerializer(fakeEventTypeMapper);
        var eventDataFactory = new EventDataFactory(serializer, fakeMetaDataFactory);

        var result = eventDataFactory.From(@event);

        result.Type.Should().Be(nameof(DummyAggregateEvent));

        var deserializedEvent = JsonConvert.DeserializeObject(
            Encoding.UTF8.GetString(result.Data.Span),
            typeof(DummyAggregateEvent),
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
