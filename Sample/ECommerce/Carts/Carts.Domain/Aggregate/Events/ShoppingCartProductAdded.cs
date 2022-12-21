using Carts.Domain.Model;
using Senf.EventSourcing.Core.Events;

namespace Carts.Domain.Aggregate.Events;

// TODO: rename to ShoppingCartProductAdded
// TODO: events should not use complex types defined outside of the event

public sealed record ShoppingCartProductAdded(
    Guid CartId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice): IAggregateEvent;

