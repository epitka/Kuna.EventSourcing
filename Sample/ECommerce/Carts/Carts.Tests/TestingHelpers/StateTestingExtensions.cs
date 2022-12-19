using Carts.Commands;
using DeepEqual.Syntax;

namespace Carts.Tests.Builders;

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
        expectedState.Version = 0;

        currentState.ShouldDeepEqual(expectedState);
    }
}
