namespace Carts.Domain.Commands;

using System;

public readonly record struct RemoveItem(
    Guid CartId,
    Guid ItemId
) : ICommand;
