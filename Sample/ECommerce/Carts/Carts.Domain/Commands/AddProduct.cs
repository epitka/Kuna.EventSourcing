using Carts.Domain.Model;
using Kuna.EventSourcing.Core.Commands;

namespace Carts.Domain.Commands;

public sealed record AddProduct(Guid CartId, ProductItem ProductItem) : ICommand
{
    public static AddProduct Create(Guid cartId, ProductItem productItem)
    {
        return new AddProduct(cartId, productItem);
    }
}