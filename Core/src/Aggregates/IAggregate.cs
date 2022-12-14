using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregate<TKey>
{
    Id<TKey> Id { get; }

    int OriginalVersion { get; }

    int Version { get; }

    IAggregateEvent[] GetPendingEvents();

    IAggregateEvent[] DequeuePendingEvents();

    void InitWith(IEnumerable<object> events);
}

public interface IAggregate<TKey, TState> : IAggregate<TKey>
    where TState : IAggregateState<TKey>, new()
{
    TState GetState();

    void InitWithState(TState state);
}