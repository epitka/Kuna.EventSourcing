using Carts.Domain.Aggregate;
using Senf.EventSourcing.Core.Aggregates;

namespace Carts.Application;

public interface IShoppingCartRepository : IAggregateRepository<Guid, ShoppingCart>
{
}
