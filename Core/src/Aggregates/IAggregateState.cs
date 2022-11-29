using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregateState
{
    public Guid Id { get; }

    public void SetId(Guid aggregateId);

    public long Version { get; set; }

    public void ApplyEvent(Event @event);
}