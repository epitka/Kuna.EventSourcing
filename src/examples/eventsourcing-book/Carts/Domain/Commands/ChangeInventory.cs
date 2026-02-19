namespace Carts.Domain.Commands;

using System;

public readonly record struct ChangeInventory(
    Guid ProductId,
    int Inventory
) : ICommand;
