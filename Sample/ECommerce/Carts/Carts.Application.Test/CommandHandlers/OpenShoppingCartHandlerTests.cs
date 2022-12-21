using System.Threading;
using System.Threading.Tasks;
using Carts.CommandHandlers;
using Carts.Domain.Commands;
using Carts.TestingInfrastructure;
using Senf.EventSourcing.Core.Ids;
using Xunit;
using static Carts.Tests.Builders.StateTestingExtensions;

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
