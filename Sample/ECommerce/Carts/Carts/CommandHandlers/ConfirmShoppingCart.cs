using Carts.ShoppingCarts;
using MediatR;

namespace Carts.Commands;


internal class ConfirmShoppingCartHandler: IHandleCommand<ConfirmShoppingCart>
{
    private readonly ICartRepository cartRepository;

    public ConfirmShoppingCartHandler(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public async Task Handle(ConfirmShoppingCart command, CancellationToken ct)
    {
        var cart = await this.cartRepository.Get(command.CartId, ct);

        cart.Process(command);

        await this.cartRepository.Save(cart, ct);
    }
}
