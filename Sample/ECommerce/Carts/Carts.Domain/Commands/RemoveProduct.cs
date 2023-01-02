using Carts.Domain.Model;
using Kuna.EventSourcing.Core.Commands;
using Kuna.EventSourcing.Core.Ids;

namespace Carts.Domain.Commands;

public record RemoveProduct(
    GuidId CartId,
    PricedProductItem ProductItem) : ICommand
{
    public static RemoveProduct Create(Guid cartId, PricedProductItem productItem)
    {
        return new RemoveProduct(cartId, productItem);
    }
}
