namespace Carts.Domain.Aggregate.Events;

public record ShoppingCartCanceled(Guid CartId, DateTime CanceledAt) : IAggregateEvent;


