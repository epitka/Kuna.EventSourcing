using System.Collections.Generic;
using System.Linq;
using Carts.Domain.Model;


namespace Carts.Services.Pricing;

public class RandomProductPriceCalculator: IProductPriceCalculator
{
    public IReadOnlyList<PricedProductItem> Calculate(params ProductItem[] productItems)
    {
        if (productItems.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(productItems));

        var random = new Random();

        return productItems
            .Select(pi =>
                PricedProductItem.Create(pi, Math.Round(new decimal(random.NextDouble() * 100),2)))
            .ToList();
    }
}
