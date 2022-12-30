using Carts.Domain.Model;
using Carts.Domain.Services;

namespace Carts.TestingInfrastructure.Stubs;

internal class FakeProductPriceCalculator: IProductPriceCalculator
{
    public const decimal FakePrice = 13;
    public IReadOnlyList<PricedProductItem> Calculate(params ProductItem[] productItems)
    {
        return productItems
            .Select(pi =>
                PricedProductItem.Create(pi, FakePrice))
            .ToList();
    }
}