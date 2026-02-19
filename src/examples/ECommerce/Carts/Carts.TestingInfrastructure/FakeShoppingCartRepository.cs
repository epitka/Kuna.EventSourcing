using Carts.Application;
using Carts.Domain.Aggregate;
using Kuna.EventSourcing.Core.TestKit;

namespace Carts.TestingInfrastructure;

public class FakeShoppingCartRepository
    : InMemoryAggregateRepository<ShoppingCart>,
      IShoppingCartRepository
{
}
