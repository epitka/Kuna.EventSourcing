namespace Carts.Domain.Commands;

using System;

public readonly record struct ClearCart(
    Guid CartId
) : ICommand;
