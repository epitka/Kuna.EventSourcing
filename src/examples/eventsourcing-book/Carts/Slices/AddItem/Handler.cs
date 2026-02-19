using System.Threading;
using System.Threading.Tasks;

namespace Carts.Slices.AddItem;

public class Handler : ICommandHandler<Domain.Commands.AddItem>
{
    private readonly ICartRepository cartRepository;

    public Handler(ICartRepository cartRepository)
    {
        this.cartRepository = cartRepository;
    }
    public async Task ExecuteAsync(Domain.Commands.AddItem command, CancellationToken ct)
    {
        var cart = await this.cartRepository.Get(command.CartId, ct);

        if (cart is null)
        {

        }
    }
}
