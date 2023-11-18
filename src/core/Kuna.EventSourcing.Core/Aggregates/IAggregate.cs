

namespace Kuna.EventSourcing.Core.Aggregates;

public interface IAggregate<TKey>
{
    Id<TKey> Id { get; }

    int OriginalVersion { get; }

    int Version { get; }

    object[] GetPendingEvents();

    object[] DequeuePendingEvents();

    void InitWith(IEnumerable<object> events);
}

public interface IAggregate<TKey, TState> : IAggregate<TKey>
    where TState : IAggregateState<TKey>, new()
{
    TState GetState();

    void InitWithState(TState state);
}