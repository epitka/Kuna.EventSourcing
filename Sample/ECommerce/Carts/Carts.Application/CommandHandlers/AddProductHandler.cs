namespace Carts.Application.CommandHandlers;

public class AddProductHandler : IHandleCommand<AddProduct>
{
    private readonly IProductPriceCalculator productPriceCalculator;
    private readonly IShoppingCartRepository shoppingCartRepository;

    public AddProductHandler(
        IShoppingCartRepository shoppingCartRepository,
        IProductPriceCalculator productPriceCalculator)
    {
        this.shoppingCartRepository = shoppingCartRepository;
        this.productPriceCalculator = productPriceCalculator;
    }

    public async Task Handle(AddProduct command, CancellationToken ct)
    {
        var cart = await this.shoppingCartRepository.Get(command.CartId, ct);

        cart.Process(command, this.productPriceCalculator);

        await this.shoppingCartRepository.Save(cart, ct);
    }
}
