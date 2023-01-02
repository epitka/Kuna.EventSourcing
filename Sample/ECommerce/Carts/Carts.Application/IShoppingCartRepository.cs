using Carts.Domain.Aggregate;
using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Application;

public interface IShoppingCartRepository : IAggregateRepository<Guid, ShoppingCart>
{
}
