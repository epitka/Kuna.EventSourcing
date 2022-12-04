using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Aggregates;

public interface IAggregate
{
    int OriginalVersion { get; }

    int Version { get; }

    Guid Id { get; }

    IEvent[] GetPendingEvents();

    IEvent[] DequeuePendingEvents();

    void InitWith(IEnumerable<IEvent> events);
}

public interface IAggregate<TState> : IAggregate
    where TState : IAggregateState, new()
{
    TState GetState();

    void InitWithState(TState state);
}