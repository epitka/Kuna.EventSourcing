using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Extensions;

namespace Senf.EventSourcing.Core.Aggregates;

public abstract class Aggregate<TKey, TState>
    : IAggregate<TKey, TState>
    where TState : AggregateState<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly Queue<IAggregateEvent> pendingEvents = new();

    public int OriginalVersion => this.CurrentState.OriginalVersion;

    public int Version => this.CurrentState.Version;

    public Id<TKey> Id => this.CurrentState.Id;

    protected TState CurrentState { get; private set; } = new();

    public void InitWithState(TState state)
    {
        _ = state ?? throw new InvalidOperationException("Cannot initialize aggreate with null state.");

        if (this.Version > -1)
        {
            throw new InvalidOperationException("State is already initialized");
        }

        this.CurrentState = state;
        this.CurrentState.SetId(state.Id.Value);
    }

    public void InitWith(IEnumerable<object> events)
    {
        this.CurrentState.InitWith(events);
    }

    /// <summary>
    /// Returns deep clone of the internal aggregate state
    /// This is expensive operation as it serializes internal state, so do not overuse.
    /// Ideally, one should never have to fetch whole state. If you need to expose some
    /// information, then create getter and clone object using DeepClone
    /// </summary>
    /// <returns></returns>
    public TState GetState()
    {
        return this.CurrentState.DeepClone();
    }

    /// <summary>
    /// returns copy of the pending events in internal queue
    /// </summary>
    /// <returns></returns>
    public IAggregateEvent[] GetPendingEvents()
    {
        return this.pendingEvents.ToArray();
    }

    public IAggregateEvent[] DequeuePendingEvents()
    {
        var toReturn = this.pendingEvents.ToArray();

        this.pendingEvents.Clear();

        return toReturn;
    }

    protected void RaiseEvent(IAggregateEvent aggregateEvent)
    {
        this.CurrentState.ApplyEvent(aggregateEvent);

        this.pendingEvents.Enqueue(aggregateEvent);
    }
}
