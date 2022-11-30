#nullable disable

using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Tests.Events
{
    public sealed record TestAggregateChanged(
        Guid Id,
        string ChangedValue) : IEvent
    {
    }
}
