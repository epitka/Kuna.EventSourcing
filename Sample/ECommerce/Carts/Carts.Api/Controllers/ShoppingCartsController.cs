using System;
using System.Threading.Tasks;
using Carts.Api.Requests;
using Carts.Commands;
using Carts.ShoppingCarts.Products;
using Microsoft.AspNetCore.Mvc;
using Senf.EventSourcing.Core.Commands;

namespace Carts.Api.Controllers;

[Route("api/[controller]")]
public class ShoppingCartsController : Controller
{
    private readonly ICommandBus commandBus;

    public ShoppingCartsController(ICommandBus commandBus)
    {
        this.commandBus = commandBus;
    }

    [HttpPost]
    public async Task<IActionResult> OpenCart([FromBody] OpenShoppingCartRequest request)
    {
        var cartId = Guid.NewGuid();

        var command = OpenShoppingCart.Create(
            cartId,
            request.ClientId
        );

        await this.commandBus.Send(command);

        return this.Created($"/api/ShoppingCarts/{cartId}", cartId);
    }

    [HttpPost("{id:guid}/products")]
    public async Task<IActionResult> AddProduct(
        Guid id,
        [FromBody]
        AddProductRequest? request)
    {
        var command = Commands.AddProduct.Create(
            id,
            ProductItem.From(
                request?.ProductItem?.ProductId,
                request?.ProductItem?.Quantity
            )
        );

        await this.commandBus.Send(command);
        return Ok();
    }


     [HttpDelete("{id:guid}/products/{productId:guid}")]
     public async Task<IActionResult> RemoveProduct(
         Guid id,
         [FromRoute]Guid? productId,
         [FromQuery]int? quantity,
         [FromQuery]decimal? unitPrice
     )
     {
         var command = Commands.RemoveProduct.Create(
             id,
             PricedProductItem.Create(
                 productId,
                 quantity,
                 unitPrice
             )
         );

         await this.commandBus.Send(command);

         return this.NoContent();
     }

     [HttpPut("{id:guid}/confirmation")]
     public async Task<IActionResult> ConfirmCart(Guid id)
     {
         var command = ConfirmShoppingCart.Create(
             id
         );

         await this.commandBus.Send(command);

         return this.Ok();
     }

    // [HttpDelete("{id}")]
     public async Task<IActionResult> CancelCart(Guid id)
     {
         var command = CancelShoppingCart.Create(id);

         await this.commandBus.Send(command);

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
