
namespace Kuna.EventSourcing.Core.Aggregates;

public abstract class Aggregate<TKey, TState>
    : IAggregate<TKey, TState>
    where TState : AggregateState<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly Queue<object> pendingEvents = new();

    /// <summary>
    /// Original version of the aggregate state, before any events were applied
    /// </summary>
    public int OriginalVersion => this.CurrentState.OriginalVersion;

    /// <summary>
    /// Version of the aggregate state, after all event(s) are applied
    /// </summary>
    public int Version => this.CurrentState.Version;

    /// <summary>
    /// Id of the aggregate state
    /// </summary>
    public Id<TKey> Id => this.CurrentState.Id;

    /// <summary>
    /// Current state of the aggregate
    /// </summary>
    protected TState CurrentState { get; private set; } = new();

    /// <summary>
    /// Initializes aggregate with state. This is extensivelly used tests or when loading snapshot of the aggregate from the backing store/>
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

    /// <summary>
    /// Initilizet aggregate with events. This is used when loading aggregate from the backing store
    /// </summary>
    /// <param name="events"></param>
    public void InitWith(IEnumerable<object> events)
    {
        this.CurrentState.InitWith(events);
    }

    /// <summary>
    /// Returns internal aggregate state. DO NOT mutate state directly.
    /// Used extensively in tests to assert mutation.
    /// </summary>
    /// <returns></returns>
    public TState GetState()
    {
        return this.CurrentState;
    }

    /// <summary>
    /// Returns copy of the pending events in internal queue
    /// </summary>
    /// <returns></returns>
    public object[] GetPendingEvents()
    {
        return this.pendingEvents.ToArray();
    }

    /// <summary>
    /// Dequeues pending events and returns them as array, basically clearing the pending events queue.
    /// This is used to save events to the backing store.
    /// </summary>
    public object[] DequeuePendingEvents()
    {
        var toReturn = this.pendingEvents.ToArray();

        this.pendingEvents.Clear();

        return toReturn;
    }

    /// <summary>
    /// Mutates internal state by calling Apply method on the state and adds event to the pending events queue
    /// </summary>
    /// <param name="aggregateEvent"></param>

    protected void RaiseEvent(IAggregateEvent aggregateEvent)
    {
        this.CurrentState.ApplyEvent(aggregateEvent);

        this.pendingEvents.Enqueue(aggregateEvent);
    }
}
