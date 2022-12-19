using Carts.Commands;

namespace Carts.CommandHandlers;

public class HandleCancelShoppingCart: IHandleCommand<CancelShoppingCart>
{
    private readonly IShoppingCartRepository shoppingCartRepository;

    public HandleCancelShoppingCart(IShoppingCartRepository shoppingCartRepository)
    {
        this.shoppingCartRepository = shoppingCartRepository;
    }

    public async Task Handle(CancelShoppingCart command, CancellationToken ct)
    {

        var cart = await this.shoppingCartRepository.Get(command.CartId, ct);

        cart.Process(command);

        await this.shoppingCartRepository.Save(cart, ct);
    }
}
