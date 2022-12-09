namespace Senf.EventSourcing.Core;

public interface IHandleEvent<in T>
where T : class
{
    Task Handle(T @event, CancellationToken ct);
}
