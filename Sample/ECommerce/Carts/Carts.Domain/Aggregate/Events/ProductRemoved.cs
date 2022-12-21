using Carts.Domain.Model;
using Senf.EventSourcing.Core.Events;

namespace Carts.Domain.Aggregate.Events;

// TODO: rename to ShoppingCartProductRemoved
public record ProductRemoved(Guid CartId, PricedProductItem ProductItem) : IAggregateEvent;

