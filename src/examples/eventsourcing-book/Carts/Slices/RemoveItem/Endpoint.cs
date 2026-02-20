using System;
using System.Threading;
using System.Threading.Tasks;

namespace Carts.Slices.RemoveItem;

public sealed class Endpoint : Endpoint<Request>
{
    public override void Configure()
    {
        this.Post("/removeitem/{cartId}");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        _ = new Domain.Commands.RemoveItem(
                CartId: req.CartId,
                ItemId: req.ItemId)
            .ExecuteAsync(ct);

        await this.Send.OkAsync(ct);
    }
}

public readonly record struct Request(Guid CartId, Guid ItemId);

