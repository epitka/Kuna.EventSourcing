#nullable disable
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;

namespace Kuna.EventSourcing.Core.Tests.Events
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Name"></param>
    public sealed record TestAggregateCreated(
        Guid Id,
        string Name);

}
