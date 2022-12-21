using Carts.Domain.Aggregate;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.EventStore;

namespace Carts.Infrastructure;

public sealed class ShoppingCartRepository
    : AggregateRepository<Guid, ShoppingCart>,
      IShoppingCartRepository
{
    public ShoppingCartRepository(
        IAggregateStreamReader streamReader,
        IAggregateStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    public override string StreamPrefix => "cart-";
}