using Carts.Domain.Commands;
using Carts.Infrastructure.Commands;

namespace Carts.Application.CommandHandlers;

public class CancelShoppingCartHandler: IHandleCommand<CancelShoppingCart>
{
    private readonly IShoppingCartRepository shoppingCartRepository;

    public CancelShoppingCartHandler(IShoppingCartRepository shoppingCartRepository)
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
