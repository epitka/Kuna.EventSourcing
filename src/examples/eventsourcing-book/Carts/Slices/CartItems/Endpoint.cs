using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;

namespace Carts.Slices.CartItems;

public sealed class Endpoint : EndpointWithoutRequest<ReadModel>
{
    public override void Configure()
    {
        this.Get("/{cartId}/cartitems");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var cartId = this.Route<Guid>("cartId");

        var result = await new Query(cartId).ExecuteAsync(ct);

        await this.Send.OkAsync(result, ct);
    }
}
