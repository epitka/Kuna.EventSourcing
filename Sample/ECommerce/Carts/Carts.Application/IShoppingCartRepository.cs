using Carts.Domain.Aggregate;
using Senf.EventSourcing.Core.Aggregates;

namespace Carts;

public interface IShoppingCartRepository : IAggregateRepository<Guid, ShoppingCart>
{
}
