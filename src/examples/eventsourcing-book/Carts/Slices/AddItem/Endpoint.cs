using System;
using System.Threading;
using System.Threading.Tasks;

namespace Carts.Slices.AddItem;

public class Endpoint : Endpoint<Request>
{
    public override void Configure()
    {
        this.Post("/additem/{cartId}");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // map request to domain command. Often there is 1:1, but sometimes they are not
        // making it explicit here that these 2 are different things.

        _ = await new Domain.Commands.AddItem(
                CartId: Guid.NewGuid(),
                ItemId: req.ItemId,
                Price: req.Price,
                Image: req.Image,
                TotalPrice: req.TotalPrice,
                Description: req.Description,
                ProductId: req.ProductId)
            .ExecuteAsync(ct);

        await this.Send.OkAsync();
    }
}

public readonly record struct Request(
    Guid CartId,
    string Description,
    string Image,
    double Price,
    double TotalPrice,
    Guid ItemId,
    Guid ProductId);
