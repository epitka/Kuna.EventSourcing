using Kuna.EventSourcing.Core.Events;

namespace Carts.Domain.Aggregate.Events;

public record ShoppingCartConfirmed(Guid CartId, DateTime ConfirmedAt) : IAggregateEvent;