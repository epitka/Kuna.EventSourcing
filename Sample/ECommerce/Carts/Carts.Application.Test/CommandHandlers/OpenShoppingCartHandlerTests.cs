using System.Threading;
using System.Threading.Tasks;
using Carts.Application;
using Carts.Application.CommandHandlers;
using Carts.Domain.Commands;
using Carts.TestingInfrastructure;
using FluentAssertions;
using Senf.EventSourcing.Core.Ids;
using Xunit;
using static Carts.TestingInfrastructure.StateTestingExtensions;

namespace Carts.Tests.CommandHandlers;

public class OpenShoppingCartHandlerTests
{
    [Fact]
    public async Task Should_Create_ShoppingCart()
    {
        // TODO: add example of using ContainerDrivenTest
        var fakeRepository = new FakeShoppingCartRepository();

        var cmd = new OpenShoppingCart(GuidId.Create(), GuidId.Create());

        var handler = new OpenShoppingCartHandler(fakeRepository);

        await handler.Handle(cmd, CancellationToken.None);

        var cart = await fakeRepository.Get(cmd.CartId, CancellationToken.None);

        cart.VerifyStateAfter(cmd);
    }
}

// example of how to use container to resolve dependencies
// so it is easier to bootstrap tests
public class OpenShoppingCartHandlerTests2 : CommandHandlerTest
{
    [Fact]
    public async Task Should_Create_ShoppingCart()
    {
        var cmd = new OpenShoppingCart(GuidId.Create(), GuidId.Create());

        this.Services.Should().NotBeNull();

        var handler = this.GetRequiredService<OpenShoppingCartHandler>();

        await handler.Handle(cmd, CancellationToken.None);

        var repository = this.GetRequiredService<IShoppingCartRepository>();

        var cart = await repository.Get(cmd.CartId, CancellationToken.None);

        cart.VerifyStateAfter(cmd);
    }
}
