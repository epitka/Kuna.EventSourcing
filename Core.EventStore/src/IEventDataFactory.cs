using System.Text;
using EventStore.Client;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore;

public interface IEventDataFactory
{
    public EventData From(
        IEvent @event);
}

public class EventDataFactory : IEventDataFactory
{
    private readonly IEventSerializer serializer;
    private readonly IEventMetadataFactory metadataFactory;

    public EventDataFactory(
        IEventSerializer serializer,
        IEventMetadataFactory metadataFactory)
    {
        this.serializer = serializer;
        this.metadataFactory = metadataFactory;
    }

    public EventData From(
        IEvent @event)
    {
        var eventId = Uuid.NewUuid();

        // TODO: can we avoid constant reflection here ???
        var eventType = @event.GetType();

        var data = this.serializer.Serialize(@event);
        var metadata = this.serializer.Serialize(this.metadataFactory.Get());

        return new EventData(eventId, eventType.Name, data, metadata);
    }
}
