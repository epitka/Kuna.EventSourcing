namespace Kuna.EventSourcing.Core.Aggregates;

public abstract class AggregateState<TKey> : IAggregateState<TKey>
where TKey : IEquatable<TKey>

{
    public Id<TKey> Id { get; private set; } = default!;

    /// <summary>
    /// Each state is identified by Id. Id is immutable and can be set only once. Operation is idempotent.
    /// </summary>
    /// <param name="aggregateId"></param>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Current version of the state
    /// </summary>
    public int Version { get; set; } = -1;

    /// <summary>
    /// Original version of the state when it was initialized or loaded from the backing store
    /// </summary>
    public int OriginalVersion { get; set; } = -1;

    /// <summary>
    /// Used to initialize state with events. It will sequentially apply events and mutate state.
    /// </summary>
    /// <param name="events"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void InitWith(IEnumerable<object> events)
    {
        if (this.Version > -1)
        {
            throw new InvalidOperationException("State is already initialized");
        }

        foreach (var @event in events)
        {
            this.ApplyEvent(@event);
        }

        this.OriginalVersion = this.Version;
    }


    /// <summary>
    /// By convention, methods that mutate state must be named Apply
    /// </summary>
    /// <param name="event"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ApplyEvent(object @event)
    {
        ((dynamic)this).Apply((dynamic)@event);

        this.Version++;
    }
}
