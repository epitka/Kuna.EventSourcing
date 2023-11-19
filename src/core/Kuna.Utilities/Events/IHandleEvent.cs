namespace Kuna.Utilities.Events;

/// <summary>
/// Marker interface used to identify handlers of events.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleEvent<TEvent>
{
    Task Handle(TEvent @event, CancellationToken ct);
}
