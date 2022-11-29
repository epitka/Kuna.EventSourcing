using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregate
{
    long Version { get; }

    Guid Id { get; }

    IEnumerable<Event> GetPendingEvents();

    void ClearPendingEvents();

    void InitWith(IEnumerable<Event> events);
}

public interface IAggregate<TState> : IAggregate
    where TState : IAggregateState, new()
{
    TState GetState();

    void InitWithState(TState state);
}