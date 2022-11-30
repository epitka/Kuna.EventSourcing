using System.Text;
using EventStore.Client;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore;

public interface IEventSerializer
{
    Event? Deserialize(ResolvedEvent @event);

    byte[] Serialize(object obj);
}

public class JsonEventSerializer : IEventSerializer
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.None,
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Include,
    };

    private readonly IEventTypeMapper eventTypeMapper;

    public JsonEventSerializer(IEventTypeMapper eventTypeMapper)
    {
        this.eventTypeMapper = eventTypeMapper;
    }

    public Event? Deserialize(ResolvedEvent resolvedEvent)
    {
        var eventType = this.eventTypeMapper.MapFrom(resolvedEvent.Event?.EventType ?? string.Empty);

        if (eventType == null)
        {
            return null;
        }

        if (resolvedEvent.Event == null)
        {
            return null;
        }

        if (JsonConvert.DeserializeObject(
                Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span),
                eventType,
                SerializerSettings
            ) is not Event @event)
        {
            return null;
        }

        @event.Version = Convert.ToInt64(resolvedEvent.Event.EventNumber);

        return @event;
    }

    public byte[] Serialize(object obj)
    {
        // TODO: run benchmarks, probably not the fastest way to serialize
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, SerializerSettings));
    }
}
