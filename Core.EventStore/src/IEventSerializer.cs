using System.Text;
using EventStore.Client;
using Newtonsoft.Json;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore;

public interface IEventSerializer
{
    IEvent? Deserialize(ResolvedEvent @event);

    IDictionary<string, string> DeserializeMetaData(ResolvedEvent resolvedEvent);

    byte[] Serialize(object obj);
}

public class JsonEventSerializer : IEventSerializer
{
    public static readonly JsonSerializerSettings SerializerSettings = new()
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

    public IEvent? Deserialize(ResolvedEvent resolvedEvent)
    {
        if (resolvedEvent.Event == null)
        {
            return null;
        }

        var eventType = this.eventTypeMapper.MapFrom(resolvedEvent.Event.EventType ?? string.Empty);

        if (JsonConvert.DeserializeObject(
                Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span),
                eventType,
                SerializerSettings
            ) is not IEvent @event)
        {
            return null;
        }

        return @event;
    }

    public IDictionary<string, string> DeserializeMetaData(ResolvedEvent resolvedEvent)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                   Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span),
                   SerializerSettings)
               ?? new Dictionary<string, string>();
    }

    public byte[] Serialize(object obj)
    {
        // TODO: run benchmarks, probably not the fastest way to serialize
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, SerializerSettings));
    }
}
