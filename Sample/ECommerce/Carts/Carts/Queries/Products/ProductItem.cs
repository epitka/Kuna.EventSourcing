namespace Carts.ShoppingCarts.Products;

public sealed record ProductItem
{
    public Guid ProductId { get; }

    public int Quantity { get; }

    private ProductItem(GuidId productId, int quantity)
    {
        this.ProductId = productId;
        this.Quantity = quantity;
    }

    public static ProductItem From(GuidId productId, int? quantity)
    {
        return quantity switch
        {
            null  => throw new ArgumentNullException(nameof(quantity)),
            <= 0  => throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity has to be a positive number"),
            var _ => new ProductItem(productId.Value, quantity.Value),
        };
    }

    public ProductItem MergeWith(ProductItem productItem)
    {
        if (!this.MatchesProduct(productItem))
            throw new ArgumentException("Product does not match.");

        return From(this.ProductId, this.Quantity + productItem.Quantity);
    }

    public ProductItem Subtract(ProductItem productItem)
    {
        if (!this.MatchesProduct(productItem))
            throw new ArgumentException("Product does not match.");

        return From(this.ProductId, this.Quantity - productItem.Quantity);
    }

    public bool MatchesProduct(ProductItem productItem)
    {
        return this.ProductId == productItem.ProductId;
    }

    public bool HasEnough(int quantity)
    {
        return this.Quantity >= quantity;
    }

    public bool HasTheSameQuantity(ProductItem productItem)
    {
        return this.Quantity == productItem.Quantity;
    }
}
