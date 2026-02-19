using System;
using ICommand = FastEndpoints.ICommand;

namespace Carts.Domain.Commands;

public readonly record struct CreateCart(Guid CartId) : ICommand;
