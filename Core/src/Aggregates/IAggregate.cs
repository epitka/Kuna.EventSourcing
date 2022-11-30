using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregate
{
    long? Version { get; }

    Guid Id { get; }

    IEnumerable<IEvent> GetPendingEvents();

    IEvent[] DequeuePendingEvents();

    void InitWith(IEnumerable<IEvent> events);
}

public interface IAggregate<TState> : IAggregate
    where TState : IAggregateState, new()
{
    TState GetState();

    void InitWithState(TState state);
}