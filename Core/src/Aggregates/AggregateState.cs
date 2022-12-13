using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Exceptions;

namespace Senf.EventSourcing.Core.Aggregates;

public abstract class AggregateState<TKey> : IAggregateState<TKey>
where TKey : IEquatable<TKey>

{
    public Id<TKey> Id { get; private set; } = default!;

    public void SetId(TKey aggregateId)
    {
        var id = new Id<TKey>(aggregateId);

        if (this.Id != null
            && !this.Id.Equals(id))
        {
            throw new InvalidOperationException("Id already set, cannot change identity of the aggreate");
        }

        this.Id = id;
    }

    public int Version { get; set; } = -1;

    public int OriginalVersion { get; set; } = -1;

    /// <summary>
    /// By convention, methods that mutate state must be named Apply
    /// </summary>
    /// <param name="aggregateEvent"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ApplyEvent(object aggregateEvent)
    {
        ((dynamic)this).Apply((dynamic)aggregateEvent);

        this.Version++;
    }
}
