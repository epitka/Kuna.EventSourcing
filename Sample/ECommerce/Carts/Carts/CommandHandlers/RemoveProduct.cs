namespace Carts.Commands;

internal class RemoveProductHandler: IHandleCommand<RemoveProduct>
{
    private readonly ICartRepository cartRepository;

    public RemoveProductHandler(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public async Task Handle(RemoveProduct command, CancellationToken cancellationToken)
    {
        var cart = await this.cartRepository.Get(command.CartId, cancellationToken);

        cart.Process(command);

        await this.cartRepository.Save(cart, cancellationToken);
    }
}
