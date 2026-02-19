using System;

namespace Carts.Slices.AddItem;

public record Request(
    Guid CartId,
    string Description,
    string Image,
    double Price,
    double TotalPrice,
    Guid ItemId,
    Guid ProductId);
