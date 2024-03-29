using Carts.Domain.Commands;

namespace Carts.Application.CommandHandlers;

public class RemoveProductHandler: IHandleCommand<RemoveProduct>
{
    private readonly IShoppingCartRepository shoppingCartRepository;

    public RemoveProductHandler(IShoppingCartRepository shoppingCartRepository)
    {
        this.shoppingCartRepository = shoppingCartRepository;
    }

    public async Task Handle(RemoveProduct command, CancellationToken ct)
    {
        var cart = await this.shoppingCartRepository.Get(command.CartId, ct);

        cart.Process(command);

        await this.shoppingCartRepository.Save(cart, ct);
    }
}
