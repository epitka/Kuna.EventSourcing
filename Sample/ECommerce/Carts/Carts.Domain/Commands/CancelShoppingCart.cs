namespace Carts.Domain.Commands;

public sealed record CancelShoppingCart(GuidId CartId)
    : ICommand
{
    public static CancelShoppingCart Create(Guid cartId)
    {
        return new CancelShoppingCart(cartId);
    }
}
