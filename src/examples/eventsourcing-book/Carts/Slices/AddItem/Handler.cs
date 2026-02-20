using System;
using System.Threading;
using System.Threading.Tasks;
using Carts.Domain;
using Kuna.EventSourcing.Core.Exceptions;

namespace Carts.Slices.AddItem;

public class Handler : ICommandHandler<Domain.Commands.AddItem>
{
    private readonly ICartRepository cartRepository;
    private readonly Fingerprint fingerprint;

    public Handler(ICartRepository cartRepository, Fingerprint fingerprint)
    {
        this.cartRepository = cartRepository;
        this.fingerprint = fingerprint;
    }

    public async Task ExecuteAsync(Domain.Commands.AddItem command, CancellationToken ct)
    {
        CartAggregate cart;

        try
        {
            cart = await this.cartRepository.Get(command.CartId, ct);
        }
        catch (AggregateNotFoundException<CartAggregate>)
        {
            cart = new CartAggregate();
        }

        cart.Handle(command, this.fingerprint);
    }
}
