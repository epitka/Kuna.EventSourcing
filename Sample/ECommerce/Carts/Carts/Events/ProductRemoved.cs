using Carts.ShoppingCarts.Products;
using Senf.EventSourcing.Core.Events;

namespace Carts.Events;

// TODO: rename to ShoppingCartProductRemoved
public record ProductRemoved(GuidId CartId, PricedProductItem ProductItem) : IAggregateEvent;

