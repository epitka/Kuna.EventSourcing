using System;

namespace Carts.Domain.Models;

public record CartItemId(Guid Value)
{
    public static implicit operator Guid(CartItemId cartItemId) => cartItemId.Value;
    public static implicit operator CartItemId(Guid value) => new(value);
}

