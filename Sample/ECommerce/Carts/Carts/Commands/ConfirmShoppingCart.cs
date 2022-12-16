using Carts.ShoppingCarts;
using Marten.Events.CodeGeneration;
using MediatR;

namespace Carts.Commands;

public record ConfirmShoppingCart(GuidId CartId) : ICommand
{
    public static ConfirmShoppingCart Create(Guid cartId)
    {
        return new ConfirmShoppingCart(cartId);
    }
}

