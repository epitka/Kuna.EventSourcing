namespace Senf.EventSourcing.Core.Events;

public interface IEventBus
{
    void Publish(IEvent @event, CancellationToken ct);
}
