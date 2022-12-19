using System;
using Carts.Events;
using Carts.ShoppingCarts.Products;
using Senf.EventSourcing.Core.Ids;
using Senf.EventSourcing.Testing;

namespace Carts.Tests.Builders;

public class ShoppingCartBuilder : AggregateBuilder<ShoppingCart, ShoppingCart.State, Guid>
{
    public static ShoppingCartBuilder With_ShoppingCartOpened(GuidId? cartId, GuidId? clientId)
    {
        var state = new ShoppingCart.State();

        // TODO add events factory
        return (ShoppingCartBuilder)Init(state, new ShoppingCartOpened(cartId ?? GuidId.Create(), clientId ?? GuidId.Create()));
    }

    public ShoppingCartBuilder With_ProductAdded(ProductItem? productItem = null, decimal unitPrice = 1)
    {
        var item = productItem ?? ProductItem.From(GuidId.Create(), 10);

        var pricedItem = PricedProductItem.Create(item.ProductId, item.Quantity, unitPrice);

        this.With(new ProductAdded(this.aggregateState.Id.Value, pricedItem));

        return this;
    }

    public ShoppingCartBuilder With_ProductRemoved(ProductItem? productItem = null, decimal unitPrice = 1)
    {
        var item = productItem ?? ProductItem.From(GuidId.Create(), 10);

        var pricedItem = PricedProductItem.Create(item.ProductId, item.Quantity, unitPrice);

        this.With(new ProductRemoved(this.aggregateState.Id.Value, pricedItem));

        return this;
    }

    public ShoppingCartBuilder With_ShoppingCartCancelled()
    {
        this.With(new ShoppingCartCanceled(this.aggregateState.Id.Value, DateTime.Now));

        return this;
    }

    public ShoppingCartBuilder With_ShoppingCartConfirmed()
    {
        this.With(new ShoppingCartConfirmed(this.aggregateState.Id.Value, DateTime.Now));

        return this;
    }
}
