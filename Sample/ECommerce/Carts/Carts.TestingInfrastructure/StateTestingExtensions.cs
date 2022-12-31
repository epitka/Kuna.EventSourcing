using Carts.Domain.Aggregate;
using Carts.Domain.Commands;
using Carts.Domain.Model;
using Senf.EventSourcing.Testing;

namespace Carts.TestingInfrastructure;

public static class StateTestingExtensions
{
    public static void VerifyStateAfter(this ShoppingCart sut, OpenShoppingCart command)
    {
        var currentState = sut.GetState();

        var expectedState = new ShoppingCart.State()
        {
            ClientId = command.ClientId,
            Status = ShoppingCartStatus.Pending,
        };

        expectedState.SetId(command.CartId);

        currentState.ShouldBeDeepEqualTo(expectedState);
    }

    public static void VerifyStateAfter(this ShoppingCart sut, AddProduct command, ShoppingCart.State beforeState, PricedProductItem pricedProductItem)
    {
        var currentState = sut.GetState();

        var expectedState = beforeState;

        expectedState.ProductItems.Add(pricedProductItem);

        currentState.ShouldBeDeepEqualTo(expectedState);
    }
}
