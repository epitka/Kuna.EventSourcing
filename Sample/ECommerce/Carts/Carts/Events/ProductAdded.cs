
using Senf.EventSourcing.Core.Events;

namespace Carts.Events;

// TODO: rename to ShoppingCartProductAdded

public sealed record ProductAdded(GuidId CartId, PricedProductItem ProductItem) : IAggregateEvent;
