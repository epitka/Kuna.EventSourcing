using System.Reflection;
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore;

public interface IEventTypeMapper
{
    Type MapFrom(string name);
}

public class EventTypeMapper : IEventTypeMapper
{
    private readonly IDictionary<string, Type> typeMap;

    public EventTypeMapper(IEnumerable<Assembly> assembliesToScan)
    {
        /*if (assembliesToScan == null
            || assembliesToScan.Any() == false)
        {
            throw new ArgumentNullException()
        }*/

        var eventType = typeof(IEvent);

        this.typeMap = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(i => i.GetTypes())
                                .Where(x => eventType.IsAssignableFrom(x))
                                .ToDictionary(x => x.Name);
    }

    public Type MapFrom(string name)
    {
        return !this.typeMap.TryGetValue(name.Trim(), out var eventType)
            ?  throw new InvalidOperationException($"Missing event type definition for event type name {name}")
            : eventType;
    }
}
