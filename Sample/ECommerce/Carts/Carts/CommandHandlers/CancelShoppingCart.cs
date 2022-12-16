using Carts.ShoppingCarts;
using MediatR;

namespace Carts.Commands;

public class HandleCancelShoppingCart: IHandleCommand<CancelShoppingCart>
{
    private readonly ICartRepository cartRepository;

    public HandleCancelShoppingCart(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public async Task Handle(CancelShoppingCart command, CancellationToken ct)
    {

        var cart = await this.cartRepository.Get(command.CartId, ct);

        cart.Process(command);

        await this.cartRepository.Save(cart, ct);
    }
}
