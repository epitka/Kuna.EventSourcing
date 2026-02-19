using System;

namespace Carts.Domain.Models;

public record ProductId(Guid Value)
{
    public static implicit operator Guid(ProductId productId) => productId.Value;
    public static implicit operator ProductId(Guid value) => new(value);
}

