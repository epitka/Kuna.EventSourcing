namespace Carts.Domain.Commands;

using System;

public readonly record struct AddItem(
    Guid CartId,
    string Description,
    string Image,
    double Price,
    double TotalPrice,
    Guid ItemId,
    Guid ProductId
) : ICommand;
