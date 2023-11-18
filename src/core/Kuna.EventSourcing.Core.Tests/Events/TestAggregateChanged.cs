#nullable disable

using Kuna.EventSourcing.Core.Aggregates;


namespace Kuna.EventSourcing.Core.Tests.Events
{
    public sealed record TestAggregateChanged(
        Guid Id,
        string ChangedValue)
    {
    }
}
