using Kuna.EventSourcing.Core.Commands;
using Kuna.EventSourcing.Core.Ids;

namespace Carts.Domain.Commands;

public record ConfirmShoppingCart(GuidId CartId) : ICommand
{
    public static ConfirmShoppingCart Create(Guid cartId)
    {
        return new ConfirmShoppingCart(cartId);
    }
}

