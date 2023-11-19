namespace Carts.Domain.Aggregate.Events;

public readonly record struct ShoppingCartProductRemoved(
    Guid CartId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice): IAggregateEvent;

