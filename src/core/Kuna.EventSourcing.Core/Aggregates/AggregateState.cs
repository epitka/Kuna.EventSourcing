using Microsoft.Extensions.Logging;
using static Kuna.EventSourcing.Core.StateMutator;


namespace Kuna.EventSourcing.Core.Aggregates;

/// <summary>
/// Holds internal state of the aggregate typed by the generic parameter representing type of the aggregate Id, such as Guid, string, int
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class AggregateState<TKey> : IAggregateState<TKey>
where TKey : IEquatable<TKey>

{
    public Id<TKey> Id { get; private set; } = default!;

    /// <summary>
    /// Each state is identified by Id. Id is immutable and can be set only once. Operation is idempotent.
    /// This is usually called when applying first event to the state.
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

        var v = this.Version;

        Mutate(this, ref v, events);

        this.Version = v;
        this.OriginalVersion = this.Version;
    }


    /// <summary>
    /// By convention, methods that mutate state must be named Apply
    /// </summary>
    /// <param name="event"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ApplyEvent(object @event)
    {
        var v = this.Version;

        Mutate(this, ref v, @event);

        this.Version = v;
    }
}
