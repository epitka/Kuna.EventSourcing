using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Domain.Aggregate.Events;

public readonly record struct ShoppingCartConfirmed(Guid CartId, DateTime ConfirmedAt) : IAggregateEvent;