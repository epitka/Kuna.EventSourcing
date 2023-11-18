namespace Carts.Domain.Aggregate.Events;

public record ShoppingCartProductRemoved(
    Guid CartId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice): IAggregateEvent;

