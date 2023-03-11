using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Domain.Aggregate.Events;

public sealed record ShoppingCartProductAdded(
    Guid CartId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice): IAggregateEvent;

