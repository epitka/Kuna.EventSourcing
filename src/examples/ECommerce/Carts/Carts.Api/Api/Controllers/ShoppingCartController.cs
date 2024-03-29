using Carts.Api.Requests;
using Carts.Domain.Commands;
using Carts.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Carts.Api.Controllers;

[Route("api/[controller]")]
public class ShoppingCartsController : Controller
{
    private readonly ICommandDispatcher commandDispatcher;

    public ShoppingCartsController(ICommandDispatcher commandDispatcher)
    {
        this.commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> OpenCart([FromBody] OpenShoppingCartRequest request, CancellationToken ct)
    {
        var cartId = Guid.NewGuid();

        var command = new OpenShoppingCart(cartId, request.ClientId);

        await this.commandDispatcher.Send(command, ct);

        return this.Created($"/api/ShoppingCarts/{cartId}", cartId);
    }

    [HttpPost("{id:guid}/products")]
    public async Task<IActionResult> AddProduct(
        Guid id,
        [FromBody]
        AddProductRequest? request,
        CancellationToken ct)
    {
        if (request == null)
        {
            return this.BadRequest("Missing request");
        }

        var command = new AddProduct(
            id,
            ProductItem.From(
                request.ProductItem.ProductId,
                request?.ProductItem?.Quantity
            )
        );

        await this.commandDispatcher.Send(command, ct);

        return this.Ok();
    }


    [HttpDelete("{id:guid}/products/{productId:guid}")]
    public async Task<IActionResult> RemoveProduct(
        Guid id,
        [FromRoute] Guid? productId,
        [FromQuery] int? quantity,
        [FromQuery] decimal? unitPrice,
        CancellationToken ct)
    {
        var command = new RemoveProduct(
            id,
            PricedProductItem.Create(
                productId,
                quantity,
                unitPrice
            )
        );

        await this.commandDispatcher.Send(command, ct);

        return this.NoContent();
    }

    [HttpPut("{id:guid}/confirmation")]
    public async Task<IActionResult> ConfirmCart(Guid id, CancellationToken ct)
    {
        var command = new ConfirmShoppingCart(id);

        await this.commandDispatcher.Send(command, ct);

        return this.Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelCart(Guid id, CancellationToken ct)
    {
        var command = CancelShoppingCart.Create(id);

        await this.commandDispatcher.Send(command, ct);

        return this.Ok();
    }
}