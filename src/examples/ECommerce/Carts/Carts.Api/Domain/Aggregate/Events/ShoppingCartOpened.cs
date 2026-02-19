using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Domain.Aggregate.Events;

public readonly record struct ShoppingCartOpened(Guid CartId, Guid ClientId) : IAggregateEvent;
