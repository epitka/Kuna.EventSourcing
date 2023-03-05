using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;

namespace Kuna.EventSourcing.Core.EventStore.Tests.TestingHelpers;

public record TestAggregateEvent(Guid Id, string Value) : IAggregateEvent
{
}
