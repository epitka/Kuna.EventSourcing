using Carts.Domain.Model;

namespace Carts.Domain.Commands;

public sealed record AddProduct(Guid CartId, ProductItem ProductItem) : ICommand
{
    public static AddProduct Create(Guid cartId, ProductItem productItem)
    {
        return new AddProduct(cartId, productItem);
    }
}