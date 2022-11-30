using System.Reflection;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore;

public interface IEventTypeMapper
{
    Type? MapFrom(string name);
}

public class EventTypeMapper : IEventTypeMapper
{
    private readonly IDictionary<string, Type> typeMap;

    public EventTypeMapper(IEnumerable<Assembly> assemblies)
    {
        var eventType = typeof(Event);

        this.typeMap = assemblies
                       .SelectMany(i => i.GetExportedTypes())
                       .Where(x => eventType.IsAssignableFrom(x))
                       .ToDictionary(x => x.Name);
    }

    public Type? MapFrom(string name)
    {
        return !this.typeMap.TryGetValue(name.Trim(), out var eventType)
            ? null
            : eventType;
    }
}