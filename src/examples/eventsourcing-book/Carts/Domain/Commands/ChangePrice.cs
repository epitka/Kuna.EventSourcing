namespace Carts.Domain.Commands;

using System;

public readonly record struct ChangePrice(
    Guid ProductId,
    double NewPrice,
    double OldPrice
) : ICommand;
