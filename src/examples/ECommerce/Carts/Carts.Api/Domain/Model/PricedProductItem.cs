using Newtonsoft.Json;

namespace Carts.Domain.Model;

public sealed class PricedProductItem
{
    public Guid ProductId => this.ProductItem.ProductId;

    public int Quantity => this.ProductItem.Quantity;

    public decimal UnitPrice { get; }

    public decimal TotalPrice => this.Quantity * this.UnitPrice;
    public ProductItem ProductItem { get; }

    [JsonConstructor]
    private PricedProductItem(Guid productId, int quantity, decimal unitPrice)
    {
        this.ProductItem = ProductItem.From(productId, quantity);
        this.UnitPrice = unitPrice;
    }

    private PricedProductItem(ProductItem productItem, decimal unitPrice)
    {
        this.ProductItem = productItem;
        this.UnitPrice = unitPrice;
    }

    public static PricedProductItem Create(GuidId productId, int? quantity, decimal? unitPrice)
    {
        return Create(
            ProductItem.From(productId, quantity),
            unitPrice
        );
    }

    public static PricedProductItem Create(ProductItem productItem, decimal? unitPrice)
    {
        return unitPrice switch
        {
            null => throw new ArgumentNullException(nameof(unitPrice)),
            <= 0 => throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Unit price has to be positive number"),
            _ => new PricedProductItem(productItem, unitPrice.Value)
        };
    }

    public bool MatchesProductAndPrice(PricedProductItem pricedProductItem)
    {
        return this.ProductId == pricedProductItem.ProductId && this.UnitPrice == pricedProductItem.UnitPrice;
    }

    public PricedProductItem MergeWith(PricedProductItem pricedProductItem)
    {
        if (!this.MatchesProductAndPrice(pricedProductItem)) throw new ArgumentException("Product or price does not match.");

        return new PricedProductItem(this.ProductItem.MergeWith(pricedProductItem.ProductItem), this.UnitPrice);
    }

    public PricedProductItem Subtract(PricedProductItem pricedProductItem)
    {
        if (!this.MatchesProductAndPrice(pricedProductItem)) throw new ArgumentException("Product or price does not match.");

        return new PricedProductItem(this.ProductItem.Subtract(pricedProductItem.ProductItem), this.UnitPrice);
    }

    public bool HasEnough(int quantity)
    {
        return this.ProductItem.HasEnough(quantity);
    }

    public bool HasTheSameQuantity(PricedProductItem pricedProductItem)
    {
        return this.ProductItem.HasTheSameQuantity(pricedProductItem.ProductItem);
    }
}
