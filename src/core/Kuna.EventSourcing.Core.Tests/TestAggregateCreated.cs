#nullable disable
using Kuna.EventSourcing.Core.Aggregates;


namespace Kuna.EventSourcing.Core.Tests;

/// <summary>
///
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
public sealed record TestAggregateCreated(
    Guid Id,
    string Name) : IAggregateEvent;