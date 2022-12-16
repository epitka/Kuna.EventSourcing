using Carts.ShoppingCarts;
using MediatR;

namespace Carts.Commands;

public sealed record OpenShoppingCart(GuidId CartId, GuidId ClientId) : ICommand
{
    public static OpenShoppingCart Create(Guid cartId, Guid clientId)
    {
        return new OpenShoppingCart(cartId, clientId);
    }
}

