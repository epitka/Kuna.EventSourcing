﻿using Carts.Domain.Aggregate;
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
}
