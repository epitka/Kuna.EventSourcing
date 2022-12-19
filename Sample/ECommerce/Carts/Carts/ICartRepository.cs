using System.Threading;
using System.Threading.Tasks;
using Carts.ShoppingCarts;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.EventStore;

namespace Carts;

public interface ICartRepository : IAggregateRepository<Guid, ShoppingCart>
{
}

public sealed class CartRepository
    : AggregateRepository<Guid, ShoppingCart>,
      ICartRepository
{
    public CartRepository(
        IAggregateStreamReader streamReader,
        IAggregateStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    public override string StreamPrefix => "cart-";
}
