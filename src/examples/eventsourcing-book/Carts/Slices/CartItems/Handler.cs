using System;
using System.Threading;
using System.Threading.Tasks;
using Kuna.EventSourcing.Core;

namespace Carts.Slices.CartItems;

public readonly record struct Query(Guid CartId) : ICommand<ReadModel>;

public class Handler : ICommandHandler<Query, ReadModel>
{
    private readonly ISession session;

    public Handler(ISession session)
    {
        this.session = session;
    }

    public async Task<ReadModel> ExecuteAsync(Query command, CancellationToken ct)
    {
        // maybe not the best way, need to think about this, this is too KurrentDB centric
        var streamId = "cart-" + command.CartId;
        var toReturn = await this.session.Project<ReadModel>(streamId, ct);
        return toReturn;
    }
}
