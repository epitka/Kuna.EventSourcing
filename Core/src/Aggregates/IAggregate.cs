using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregate<TKey>
{
    Id<TKey> Id { get; }

    int OriginalVersion { get; }

    int Version { get; }

    IEvent[] GetPendingEvents();

    IEvent[] DequeuePendingEvents();

    void InitWith(IEnumerable<IEvent> events);
}

public interface IAggregate<TKey, TState> : IAggregate<TKey>
    where TState : IAggregateState<TKey>, new()
{
    TState GetState();

    void InitWithState(TState state);
}