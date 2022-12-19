using Carts.ShoppingCarts.Products;

namespace Carts.Commands;

public sealed record AddProduct(Guid CartId, ProductItem ProductItem) : ICommand
{
    public static AddProduct Create(Guid cartId, ProductItem productItem)
    {
        return new AddProduct(cartId, productItem);
    }
}