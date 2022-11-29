using Senf.EventSourcing.Core.Events;
using Senf.EventSourcing.Core.Extensions;

namespace Senf.EventSourcing.Core.Aggregates;

public abstract class Aggregate<TState>
    : IAggregate<TState>
    where TState : AggregateState, new()
{
    private readonly Queue<Event> pendingEvents = new();

    public long Version => this.CurrentState.Version;

    public Guid Id => this.CurrentState.Id;

    protected TState CurrentState { get; private set; } = new();

    void IAggregate<TState>.InitWithState(TState state)
    {
        _ = state ?? throw new InvalidOperationException("Cannot initialize aggreate with null state.");

        if (this.CurrentState != null)
        {
            throw new InvalidOperationException("State is already initialized");
        }

        this.CurrentState = state;
        this.CurrentState.SetId(state.Id);
    }

    public void InitWith(IEnumerable<Event> events)
    {
        _ = events ?? throw new InvalidOperationException("Cannot initialize aggreate with null events.");

        if (this.Version > -1)
        {
            throw new InvalidOperationException("State is already initialized");
        }

        foreach (var @event in events)
        {
            this.CurrentState.ApplyEvent(@event);
        }
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

    public IEnumerable<Event> GetPendingEvents()
    {
        return this.pendingEvents.ToArray();
    }

    protected void RaiseEvent(Event @event)
    {
        @event.Version = this.Version + 1;

        this.CurrentState.ApplyEvent(@event);

        this.pendingEvents.Enqueue(@event);
    }
}
