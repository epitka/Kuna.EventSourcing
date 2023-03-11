#nullable disable

using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;

namespace Kuna.EventSourcing.Core.Tests.Events
{
    public sealed record TestAggregateChanged(
        Guid Id,
        string ChangedValue) : IAggregateEvent
    {
    }
}
