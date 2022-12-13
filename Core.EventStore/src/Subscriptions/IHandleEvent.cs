namespace Senf.EventSourcing.Core;

/// <summary>
/// Marker interface used to identify handlers of events
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleEvent<in T>
where T : class
{
    Task Handle(T @event, CancellationToken ct);
}
