#nullable disable

using Kuna;


#nullable disable

using Kuna.EventSourcing.Core.Aggregates;


namespace Kuna.EventSourcing.Core.Tests
{
    public sealed record TestAggregateChanged(
        Guid Id,
        string ChangedValue)
    {
    }
}
