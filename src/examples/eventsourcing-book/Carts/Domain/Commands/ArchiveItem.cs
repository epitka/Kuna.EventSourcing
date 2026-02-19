namespace Carts.Domain.Commands;

using System;

public readonly record struct ArchiveItem(
    Guid CartId,
    Guid ProductId
) : ICommand;
