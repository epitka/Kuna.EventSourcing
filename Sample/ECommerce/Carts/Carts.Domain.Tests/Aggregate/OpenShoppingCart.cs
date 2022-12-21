using System.Linq;
using Carts.Domain.Aggregate;
using Carts.Domain.Aggregate.Events;
using Carts.Domain.Commands;
using Carts.Tests.Builders;
using DeepEqual.Syntax;
using FluentAssertions;
using Senf.EventSourcing.Core.Ids;
using Xunit;
using static Carts.Tests.Builders.StateTestingExtensions;

namespace Carts.Tests.ShoppingCarts;

public class OpenShoppingCartTests
{
    [Fact]
    public void Should_Mutate_State_And_Raise_Event()
    {
        var cartId = GuidId.Create();
        var clientId = GuidId.Create();

        var command = new OpenShoppingCart(cartId, clientId);

        var cart = ShoppingCart.Process(command);

        cart.VerifyStateAfter(command);

        // verify that correct events have been raised
        var expectedEvent = new ShoppingCartOpened(cartId, clientId);
        var pendingEvents = cart.GetPendingEvents();
        pendingEvents.Length.Should().Be(1);
        var @event = pendingEvents.First();
        @event.Should().BeAssignableTo<ShoppingCartOpened>();
        @event.ShouldDeepEqual(expectedEvent);
    }
}
