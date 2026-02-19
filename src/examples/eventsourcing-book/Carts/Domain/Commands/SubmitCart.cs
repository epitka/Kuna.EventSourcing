namespace Carts.Domain.Commands;

using System;

public readonly record struct SubmitCart(
    Guid CartId
) : ICommand;
