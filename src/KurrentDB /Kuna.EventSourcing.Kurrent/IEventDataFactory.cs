using KurrentDB.Client;

namespace Kuna.EventSourcing.Kurrent;

public interface IEventDataFactory
{
    public EventData From(
        object aggregateEvent);
}

public class EventDataFactory : IEventDataFactory
{
    private readonly IEventStoreSerializer storeSerializer;
    private readonly IEventMetadataFactory metadataFactory;

    public EventDataFactory(
        IEventStoreSerializer storeSerializer,
        IEventMetadataFactory metadataFactory)
    {
        this.storeSerializer = storeSerializer;
        this.metadataFactory = metadataFactory;
    }

    public EventData From(
        object aggregateEvent)
    {
        var eventId = Uuid.NewUuid();

        // TODO: can we avoid constant reflection here ???
        // if we had static field on each event with a name of the event we could
        // forgo reflection here
        var eventType = aggregateEvent.GetType();

        var data = this.storeSerializer.Serialize(aggregateEvent);
        var metadata = this.storeSerializer.Serialize(this.metadataFactory.Get());

        return new EventData(eventId, eventType.Name, data, metadata);
    }
}
