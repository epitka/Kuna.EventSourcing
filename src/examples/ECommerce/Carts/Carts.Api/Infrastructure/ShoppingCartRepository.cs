using Carts.Application;
using Carts.Domain.Aggregate;
using Kuna.EventSourcing.EventStore;

namespace Carts.Infrastructure;

public sealed class ShoppingCartRepository
    : AggregateRepository<Guid, ShoppingCart>,
      IShoppingCartRepository
{
    public ShoppingCartRepository(
        IStreamReader streamReader,
        IStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    public override string StreamPrefix => "cart-";
}