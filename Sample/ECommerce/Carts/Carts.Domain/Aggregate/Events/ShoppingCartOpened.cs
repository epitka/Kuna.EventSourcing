
using Senf.EventSourcing.Core.Events;

namespace Carts.Domain.Aggregate.Events;

public record ShoppingCartOpened(Guid CartId, Guid ClientId) : IAggregateEvent;
