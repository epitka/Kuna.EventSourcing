using Carts.Domain.Aggregate;
using Carts.Domain.Commands;
using Carts.Infrastructure.Commands;

namespace Carts.Application.CommandHandlers;

public class OpenShoppingCartHandler: IHandleCommand<OpenShoppingCart>
{
    private readonly IShoppingCartRepository shoppingCartRepository;

    public OpenShoppingCartHandler(IShoppingCartRepository shoppingCartRepository)
    {
        this.shoppingCartRepository = shoppingCartRepository;
    }

    public async Task Handle(OpenShoppingCart command, CancellationToken ct)
    {
        var cart = ShoppingCart.Process(command);

        await this.shoppingCartRepository.Save(cart, ct);
    }
}
