using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.Aggregates;

public abstract class AggregateState : IAggregateState
{
    public Guid Id { get; private set; } = Guid.Empty;

    public void SetId(Guid aggregateId)
    {
        if (this.Id != default
            && this.Id != aggregateId)
        {
            throw new InvalidOperationException("Id already set, cannot change identity of the aggreate");
        }

        this.Id = aggregateId;
    }

    public long? Version { get; set; }

    /// <summary>
    /// By convention, methods that mutate state must be named Apply
    /// </summary>
    /// <param name="event"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ApplyEvent(Event @event)
    {
        if (@event.Version != this.Version + 1)
        {
            throw new InvalidExpectedVersionException(
                $"{@event.GetType().Name} Event v{@event.Version} cannot be applied to Aggregate state {this.GetType().Name} : {this.Id} v{this.Version}");
        }

        ((dynamic)this).Apply((dynamic)@event);

        this.Version++;
    }
}
