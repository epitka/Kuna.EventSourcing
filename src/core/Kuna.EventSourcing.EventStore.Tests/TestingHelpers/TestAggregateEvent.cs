using Kuna.EventSourcing.Core.Aggregates;

namespace Kuna.EventSourcing.EventStore.Tests.TestingHelpers;

public record TestAggregateEvent(Guid Id, string Value) : IAggregateEvent
{
}
