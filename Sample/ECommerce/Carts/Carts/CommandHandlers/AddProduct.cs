using Carts.Pricing;

namespace Carts.Commands;

internal class AddProductHandler : IHandleCommand<AddProduct>
{
    private readonly IProductPriceCalculator productPriceCalculator;
    private readonly ICartRepository cartRepository;

    public AddProductHandler(
        ICartRepository cartRepository,
        IProductPriceCalculator productPriceCalculator)
    {
        this.cartRepository = cartRepository;
        this.productPriceCalculator = productPriceCalculator;
    }

    public async Task Handle(AddProduct command, CancellationToken ct)
    {
        var cart = await this.cartRepository.Get(command.CartId, ct);

        cart.Process(command, this.productPriceCalculator);

        await this.cartRepository.Save(cart, ct);
    }
}
