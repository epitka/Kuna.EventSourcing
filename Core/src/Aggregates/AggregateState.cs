using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregateState
{
    public Guid Id { get; }

    public void SetId(Guid aggregateId);

    public long Version { get; set; }

    public void ApplyEvent(Event @event);
}

public abstract class AggregateState : IAggregateState
{
    public Guid Id { get; private set; } = Guid.Empty;

    public void SetId(Guid aggregateId)
    {
        if (this.Id != default && this.Id != aggregateId)
        {
            throw new InvalidOperationException("Id already set, cannot change identity of the aggreate");
        }

        this.Id = aggregateId;
    }

    public long Version { get; set; } = -1;

    public void ApplyEvent(Event @event)
    {
        if (@event.Version != this.Version + 1)
        {
            throw new InvalidOperationException($"{@event.GetType().Name} Event v{@event.Version} cannot be applied to Aggregate state {this.GetType().Name} : {this.Id} v{this.Version}");
        }

        ((dynamic)this).Apply((dynamic)@event);
        this.Version++;
    }
}
