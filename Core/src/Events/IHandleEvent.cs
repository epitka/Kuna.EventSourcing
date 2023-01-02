namespace Kuna.EventSourcing.Core.Events;

/// <summary>
/// Marker interface used to identify handlers of events
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleEvent<TEvent>
where TEvent : class, IAggregateEvent
{
    Task Handle(TEvent @event, CancellationToken ct);
}
