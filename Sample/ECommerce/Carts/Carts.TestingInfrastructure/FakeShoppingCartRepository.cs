using Carts.Application;
using Carts.Domain.Aggregate;
using Kuna.EventSourcing.Testing;

namespace Carts.TestingInfrastructure;

public class FakeShoppingCartRepository
    : InMemoryAggregateRepository<Guid, ShoppingCart>,
      IShoppingCartRepository
{
}
