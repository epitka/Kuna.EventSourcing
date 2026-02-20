using System;
using System.Threading;
using System.Threading.Tasks;

namespace Carts.Slices.SubmitCart;

// POST /submitcart/{aggregateId} with JSON body
public sealed class Endpoint : Endpoint<Request>
{
    public override void Configure()
    {
        this.Post("/submitcart/{aggregateId}");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        _ = new Domain.Commands.SubmitCart(CartId: req.CartId)
            .ExecuteAsync(ct);

        await this.Send.OkAsync(ct);
    }
}

public readonly record struct Request(Guid CartId);
