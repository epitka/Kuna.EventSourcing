using Carts.Domain.Model;

namespace Carts.Domain.Services;

public interface IProductPriceCalculator
{
    IReadOnlyList<PricedProductItem> Calculate(params ProductItem[] productItems);
}