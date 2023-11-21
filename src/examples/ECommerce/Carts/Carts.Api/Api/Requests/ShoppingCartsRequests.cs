namespace Carts.Api.Requests;

public sealed record OpenShoppingCartRequest(
    Guid ClientId
);

public sealed record ProductItemRequest(
    Guid? ProductId,
    int? Quantity
);

public sealed record AddProductRequest(
    ProductItemRequest? ProductItem
);

public sealed record PricedProductItemRequest(
    Guid? ProductId,
    int? Quantity,
    decimal? UnitPrice
);

public sealed record RemoveProductRequest(
    PricedProductItemRequest? ProductItem
);

public sealed record GetCartAtVersionRequest(
    Guid? CartId,
    long? Version
);
