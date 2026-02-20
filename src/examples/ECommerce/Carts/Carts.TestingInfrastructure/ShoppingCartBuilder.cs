using Carts.Domain.Aggregate;
using Carts.Domain.Aggregate.Events;
using Carts.Domain.Model;
using Kuna.EventSourcing.Core.TestKit;

namespace Carts.TestingInfrastructure;

public class ShoppingCartBuilder
{
    private AggregateBuilder<ShoppingCart, ShoppingCart.State, Guid> builder = null!;

    public static ShoppingCartBuilder With_ShoppingCartOpened(GuidId? cartId, GuidId? clientId)
    {
        var state = new ShoppingCart.State();

        var b = AggregateBuilder<ShoppingCart, ShoppingCart.State, Guid>
            .Init(state, new ShoppingCartOpened(cartId ?? GuidId.Create(), clientId ?? GuidId.Create()));

        return new ShoppingCartBuilder()
        {
            builder = b,
        };
    }

    public ShoppingCart Build()
    {
        return this.builder.Build();
    }

    public ShoppingCartBuilder With_ProductAdded(ProductItem? productItem = null, decimal unitPrice = 1)
    {
        var item = productItem ?? ProductItem.From(GuidId.Create(), 10);

        var pricedItem = PricedProductItem.Create(item.ProductId, item.Quantity, unitPrice);

        this.builder.With(
            new ShoppingCartProductAdded(
                this.builder.aggregateState.Id.Value,
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

        this.builder.With(new ShoppingCartProductRemoved(
                      this.builder.aggregateState.Id.Value,
                      pricedItem.ProductId,
                      pricedItem.Quantity,
                      pricedItem.UnitPrice,
                      pricedItem.TotalPrice));

        return this;
    }

    public ShoppingCartBuilder With_ShoppingCartCancelled()
    {
        this.builder.With(new ShoppingCartCanceled(this.builder.aggregateState.Id.Value, DateTime.Now));

        return this;
    }

    public ShoppingCartBuilder With_ShoppingCartConfirmed()
    {
        this.builder.With(new ShoppingCartConfirmed(this.builder.aggregateState.Id.Value, DateTime.Now));

        return this;
    }
}
