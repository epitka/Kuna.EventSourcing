using System;
using System.Linq;
using Carts.Commands;
using Carts.Events;
using Carts.Pricing;
using Carts.ShoppingCarts.Products;
using Senf.EventSourcing.Core.Aggregates;

namespace Carts.ShoppingCarts;

public partial class ShoppingCart : Aggregate<Guid, ShoppingCart.State>
{
    public static ShoppingCart Process(OpenShoppingCart command)
    {
        return new ShoppingCart(command.CartId, command.ClientId);
    }

    private ShoppingCart(
        GuidId id,
        GuidId clientId)
    {
        var @event = new ShoppingCartOpened(id, clientId);

        this.RaiseEvent(@event);
    }

    public ShoppingCart()
    {
    }

    public void Process(AddProduct command, IProductPriceCalculator productPriceCalculator)
    {
        if (this.CurrentState.Status != ShoppingCartStatus.Pending)
        {
            throw new InvalidOperationException($"Adding product for the cart in '{this.CurrentState.Status}' status is not allowed.");
        }

        var pricedProductItem = productPriceCalculator.Calculate(command.ProductItem).Single();

        var @event = new ProductAdded(this.Id.Value, pricedProductItem);

        this.RaiseEvent(@event);
    }

    public void Process(RemoveProduct command)
    {
        if (this.CurrentState.Status != ShoppingCartStatus.Pending)
        {
            throw new InvalidOperationException($"Removing product from the cart in '{this.CurrentState.Status}' status is not allowed.");
        }

        var productItemToBeRemoved = command.ProductItem;

        var existingProductItem = this.CurrentState.FindProductItemMatchingWith(productItemToBeRemoved);

        if (existingProductItem is null)
        {
            throw new InvalidOperationException($"Product with id `{productItemToBeRemoved.ProductId}` and price '{productItemToBeRemoved.UnitPrice}' was not found in cart.");
        }

        if (!existingProductItem.HasEnough(productItemToBeRemoved.Quantity))
            throw new InvalidOperationException(
                $"Cannot remove {productItemToBeRemoved.Quantity} items of Product with id `{productItemToBeRemoved.ProductId}` as there are only ${existingProductItem.Quantity} items in card");

        this.RaiseEvent(new ProductRemoved(this.Id.Value, productItemToBeRemoved));
    }

    public void Process(ConfirmShoppingCart command)
    {
        // TODO: it is not good idea to have DateTime.UtcNow hardcoded

        if (this.CurrentState.Status != ShoppingCartStatus.Pending)
        {
            throw new InvalidOperationException($"Confirming cart in '{this.CurrentState.Status}' status is not allowed.");
        }

        this.RaiseEvent(new ShoppingCartConfirmed(this.Id.Value, DateTime.UtcNow));
    }

    public void Process(CancelShoppingCart command)
    {
        if (this.CurrentState.Status != ShoppingCartStatus.Pending)
        {
            throw new InvalidOperationException($"Canceling cart in '{this.CurrentState.Status}' status is not allowed.");
        }

        var @event = new ShoppingCartCanceled(this.Id.Value, DateTime.UtcNow);

        this.RaiseEvent(@event);
    }
}
