using System.Collections.Concurrent;
using System.Reflection;
using Kuna.EventSourcing.Core.Events;

namespace Kuna.EventSourcing.Core.EventStore;

public interface IEventTypeMapper
{
    Type MapFrom(string name);
}

public class EventTypeMapper : IEventTypeMapper
{
    private static readonly ConcurrentDictionary<string, Type> TypeMap = new();

    /// <summary>
    /// Type registered as singleton that contains map event types
    /// </summary>
    /// <param name="assembliesToScan"></param>
    public EventTypeMapper(IEnumerable<Assembly> assembliesToScan)
    {
        if (assembliesToScan == null
            || !assembliesToScan.Any())
        {
            throw new ArgumentNullException(
                nameof(assembliesToScan),
                "No assemblies to scan for IAggregateEvent implmentations found.");
        }

        var interfaceType = typeof(IAggregateEvent);

        var eventTypes = assembliesToScan
                         .SelectMany(i => i.GetTypes())
                         .Where(x => interfaceType.IsAssignableFrom(x));

        foreach (var eventType in eventTypes)
        {
            TypeMap.TryAdd(eventType.Name, eventType);
        }
    }

    public Type MapFrom(string name)
    {
        if (TypeMap.TryGetValue(name, out var eventType))
        {
            return eventType;
        }

        throw new InvalidOperationException($"Missing event type definition for event type name {name}");
    }
}
