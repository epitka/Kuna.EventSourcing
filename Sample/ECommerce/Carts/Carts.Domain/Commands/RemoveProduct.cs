using Carts.Domain.Model;

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
