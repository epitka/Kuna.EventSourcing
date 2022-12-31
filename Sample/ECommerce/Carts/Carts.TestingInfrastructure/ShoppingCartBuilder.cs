using Carts.Domain.Aggregate;
using Carts.Domain.Aggregate.Events;
using Carts.Domain.Model;
using Senf.EventSourcing.Core.Ids;
using Senf.EventSourcing.Testing;

namespace Carts.TestingInfrastructure;

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

        this.With(
            new ShoppingCartProductAdded(
                this.aggregateState.Id.Value,
                pricedItem.ProductId,
                pricedItem.Quantity,
                pricedItem.UnitPrice,
                pricedItem.TotalPrice));

        return this;
    }

    public ShoppingCartBuilder With_ProductRemoved(ProductItem? productItem = null, decimal unitPrice = 1)
    {
        var item = productItem ?? ProductItem.From(GuidId.Create(), 10);

        var pricedItem = PricedProductItem.Create(item.ProductId, item.Quantity, unitPrice);

        this.With(new ShoppingCartProductRemoved(
                      this.aggregateState.Id.Value,
                      pricedItem.ProductId,
                      pricedItem.Quantity,
                      pricedItem.UnitPrice,
                      pricedItem.TotalPrice));

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
