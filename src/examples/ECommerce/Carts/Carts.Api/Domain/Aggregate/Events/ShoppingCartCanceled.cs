namespace Carts.Domain.Aggregate.Events;

public readonly record struct ShoppingCartCanceled(Guid CartId, DateTime CanceledAt) : IAggregateEvent;


