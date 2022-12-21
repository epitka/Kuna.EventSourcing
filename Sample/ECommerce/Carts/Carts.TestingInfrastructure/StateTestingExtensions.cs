using Carts.Domain.Aggregate;
using Carts.Domain.Commands;
using Carts.Domain.Model;
using DeepEqual.Syntax;
using Senf.EventSourcing.Testing;

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

        currentState.ShouldBeDeepEqualTo(expectedState);
    }
}
