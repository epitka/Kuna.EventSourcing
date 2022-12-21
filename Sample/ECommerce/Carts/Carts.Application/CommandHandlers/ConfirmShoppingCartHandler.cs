
namespace Carts.CommandHandlers;

public class ConfirmShoppingCartHandler: IHandleCommand<ConfirmShoppingCart>
{
    private readonly IShoppingCartRepository shoppingCartRepository;

    public ConfirmShoppingCartHandler(IShoppingCartRepository shoppingCartRepository)
    {
        this.shoppingCartRepository = shoppingCartRepository;
    }

    public async Task Handle(ConfirmShoppingCart command, CancellationToken ct)
    {
        var cart = await this.shoppingCartRepository.Get(command.CartId, ct);

        cart.Process(command);

        await this.shoppingCartRepository.Save(cart, ct);
    }
}
