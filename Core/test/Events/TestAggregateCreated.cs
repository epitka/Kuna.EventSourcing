#nullable disable
using Senf.EventSourcing.Core.Events;

namespace Senf.EventSourcing.Core.Tests.Events
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Name"></param>
    public sealed record TestAggregateCreated(
        Guid Id,
        string Name) : IEvent {}

}
