using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

public record TestAggregateEvent(Guid Id, string Value) : IAggregateEvent
{
}
