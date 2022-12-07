namespace Senf.EventSourcing.Core.Events;

public interface IEventBus
{
    Task Publish(IEvent @event, CancellationToken ct);
}
