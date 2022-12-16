using Carts.ShoppingCarts;

namespace Carts.Commands;

internal class OpenShoppingCartHandler: IHandleCommand<OpenShoppingCart>
{
    private readonly ICartRepository cartRepository;

    public OpenShoppingCartHandler(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public async Task Handle(OpenShoppingCart command, CancellationToken ct)
    {
        var cart = ShoppingCart.Process(command);

        this.cartRepository.Save(cart, ct);
    }
}
