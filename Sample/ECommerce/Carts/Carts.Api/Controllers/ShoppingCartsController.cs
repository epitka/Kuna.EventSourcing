using System;
using System.Threading;
using System.Threading.Tasks;
using Carts.Api.Requests;
using Carts.Domain.Commands;
using Carts.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Senf.EventSourcing.Core.Commands;

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
        var command = new AddProduct(
            id,
            ProductItem.From(
                request?.ProductItem?.ProductId,
                request?.ProductItem?.Quantity
            )
        );

        await this.commandDispatcher.Send(command, ct);

        return this.Ok();
    }


     [HttpDelete("{id:guid}/products/{productId:guid}")]
     public async Task<IActionResult> RemoveProduct(
         Guid id,
         [FromRoute]Guid? productId,
         [FromQuery]int? quantity,
         [FromQuery]decimal? unitPrice,
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

    // [HttpGet("{id}")]
    // public async Task<ShoppingCartDetails> Get(Guid id)
    // {
    //     var result = await queryBus.Send<GetCartById, ShoppingCartDetails>(GetCartById.Create(id));
    //
    //     Response.TrySetETagResponseHeader(result.Version);
    //
    //     return result;
    // }
    //
    // [HttpGet]
    // public async Task<PagedListResponse<ShoppingCartShortInfo>> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    // {
    //     var pagedList = await queryBus.Send<GetCarts, IPagedList<ShoppingCartShortInfo>>(GetCarts.Create(pageNumber, pageSize));
    //
    //     return pagedList.ToResponse();
    // }
    //
    //
    // [HttpGet("{id}/history")]
    // public async Task<PagedListResponse<ShoppingCartHistory>> GetHistory(Guid id)
    // {
    //     var pagedList = await queryBus.Send<GetCartHistory, IPagedList<ShoppingCartHistory>>(GetCartHistory.Create(id));
    //
    //     return pagedList.ToResponse();
    // }
    //
    // [HttpGet("{id}/versions")]
    // public Task<ShoppingCartDetails> GetVersion(Guid id, [FromQuery] GetCartAtVersionRequest? query)
    // {
    //     return queryBus.Send<GetCartAtVersion, ShoppingCartDetails>(GetCartAtVersion.Create(id, query?.Version));
    // }
}
