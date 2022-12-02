using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.EventStore.Tests.TestingHelpers;

public record TestEvent(Guid Id, string Value) : IEvent
{
}
