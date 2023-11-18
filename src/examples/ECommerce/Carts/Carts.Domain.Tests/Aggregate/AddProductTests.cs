
using Kuna.Utilities.Ids;

namespace Carts.Tests.Aggregate;

public class AddProductTests
{
    [Fact]
    public void Should_Mutate_State_And_Raise_Event()
    {
        var cartId = GuidId.Create();
        var clientId = GuidId.Create();
        var productId = GuidId.Create();

        var sut = ShoppingCartBuilder.With_ShoppingCartOpened(cartId, clientId)
                                     .Build();

        var command = new AddProduct(cartId, ProductItem.From(productId, 10));

        var productPriceCalculator = A.Fake<IProductPriceCalculator>();
        A.CallTo(() => productPriceCalculator.Calculate(command.ProductItem))
         .Returns(new[] { PricedProductItem.Create(command.ProductItem, 10) });

        var pricedProductItem = productPriceCalculator.Calculate(command.ProductItem).First();

        // capture state before applying command
        var beforeState = sut.GetState();

        sut.Process(command, productPriceCalculator);

        // verify state
        sut.VerifyStateAfter(command, beforeState, pricedProductItem);

        // verify that correct events have been raised
        var expectedEvent = new ShoppingCartProductAdded(cartId,
                                                         pricedProductItem.ProductId,
                                                         pricedProductItem.Quantity,
                                                         pricedProductItem.UnitPrice,
                                                         pricedProductItem.TotalPrice);
        var pendingEvents = sut.GetPendingEvents();
        pendingEvents.Length.Should().Be(1);
        var @event = pendingEvents.First();
        @event.Should().BeAssignableTo<ShoppingCartProductAdded>();
        @event.ShouldDeepEqual(expectedEvent);
    }
}
