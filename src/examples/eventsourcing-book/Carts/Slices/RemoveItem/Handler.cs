using System.Threading;
using System.Threading.Tasks;

namespace Carts.Slices.RemoveItem;

public class Handler : ICommandHandler<Domain.Commands.RemoveItem>
{
    private readonly ICartRepository cartRepository;

    public Handler(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }

    public async Task ExecuteAsync(Domain.Commands.RemoveItem command, CancellationToken ct)
    {
        var cart = await this.cartRepository.Get(command.CartId, ct);

        cart.Handle(command);
    }
}
