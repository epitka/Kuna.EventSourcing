using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.Core.TestKit;

public class AggregateBuilder<TAggregate, TState, TKey>
    where TAggregate : Aggregate<TKey, TState>, new()
    where TState : AggregateState<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private const string RaiseEvent = "RaiseEvent";
    private Queue<object> queuedEvents = default!;

    public TState aggregateState = default!;

    public static AggregateBuilder<TAggregate, TState, TKey> Init(TState state, object createEvent)
    {
        var builder = new AggregateBuilder<TAggregate, TState, TKey>
        {
            aggregateState = state,
            queuedEvents = new Queue<object>(4),
        };

        builder.aggregateState.ApplyEvent(createEvent);

        builder.queuedEvents.Enqueue(createEvent);

        return builder;
    }

    public static AggregateBuilder<TAggregate, TState, TKey> StartWith(TAggregate instance)
    {
        var builder = new AggregateBuilder<TAggregate, TState, TKey>
        {
            aggregateState = instance.GetState(),
            queuedEvents = new Queue<object>(4),
        };

        return builder;
    }

    public AggregateBuilder<TAggregate, TState, TKey> With(IEnumerable<object> events)
    {
        foreach (var @event in events)
        {
            this.aggregateState.ApplyEvent(@event);
            this.queuedEvents.Enqueue(@event);
        }

        return this;
    }

    public AggregateBuilder<TAggregate, TState, TKey> With(object @event)
    {
        this.aggregateState.ApplyEvent(@event);
        this.queuedEvents.Enqueue(@event);
        return this;
    }

    /// <summary>
    /// builds instance with state based on events applied as if it was fetched from db.
    /// </summary>
    /// <returns></returns>
    public TAggregate Build()
    {
        var toReturn = new TAggregate();

        toReturn.InitWithState(this.aggregateState);

        this.queuedEvents.Clear();

        return toReturn;
    }

    /// <summary>
    /// Saves aggregate to event store and returns instance of the aggregate. You can use InMemoryAggregateRepository.
    /// </summary>
    public async Task<TAggregate> BuildAndSave(IAggregateRepository<TKey, TAggregate> repository)
    {
        var aggregate = new TAggregate();

        var raiseEvent = typeof(TAggregate).GetMethod(RaiseEvent, BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var @event in this.queuedEvents)
        {
            raiseEvent!.Invoke(aggregate, new[] { @event });
        }

        await repository.Save(aggregate, CancellationToken.None)
                        .ConfigureAwait(false);

        this.queuedEvents.Dequeue();

        return aggregate;
    }
}
