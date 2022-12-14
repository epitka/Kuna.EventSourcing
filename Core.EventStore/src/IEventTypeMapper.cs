using System.Collections.Concurrent;
using System.Reflection;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore;

public interface IEventTypeMapper
{
    Type MapFrom(string name);
}

public class EventTypeMapper : IEventTypeMapper
{
    private static readonly ConcurrentDictionary<string, Type> typeMap = new();

    /// <summary>
    /// Type registered as singleton that contains map event types
    /// </summary>
    /// <param name="assembliesToScan"></param>
    public EventTypeMapper(IEnumerable<Assembly> assembliesToScan)
    {
        // TODO: uncomment once configuration extension methods have been implemented
        /*if (assembliesToScan == null
            || assembliesToScan.Any() == false)
        {
            throw new ArgumentNullException();
        }*/

        var interfaceType = typeof(IAggregateEvent);

        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                                  .SelectMany(i => i.GetTypes())
                                  .Where(x => interfaceType.IsAssignableFrom(x));

        foreach (var eventType in eventTypes)
        {
            typeMap.TryAdd(eventType.Name, eventType);
        }
    }

    public Type MapFrom(string name)
    {
        if (typeMap!.TryGetValue(name, out var eventType))
        {
            return eventType;
        }

        throw new InvalidOperationException($"Missing event type definition for event type name {name}");
    }
}
