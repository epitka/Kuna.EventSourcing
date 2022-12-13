using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core;

/// <summary>
/// Marker interface used to identify handlers of events
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleEvent<in TEvent>
where TEvent : class, IAggregateEvent
{
    Task Handle(IAggregateEvent @event, CancellationToken ct);
}
