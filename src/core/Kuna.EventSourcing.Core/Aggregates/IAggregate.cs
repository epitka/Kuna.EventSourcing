namespace Kuna.EventSourcing.Core.Aggregates;

/// <summary>
/// Type of the aggregate root Id.
/// This assumes that key will eiter be Guid, string, or number  
/// </summary>
/// <typeparam name="TKey"></typeparam>
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