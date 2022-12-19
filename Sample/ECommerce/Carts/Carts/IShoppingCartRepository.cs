using System.Threading;
using System.Threading.Tasks;
using Senf.EventSourcing.Core.Aggregates;
using Senf.EventSourcing.Core.EventStore;

namespace Carts;

public interface IShoppingCartRepository : IAggregateRepository<Guid, ShoppingCart>
{
}

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
