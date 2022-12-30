using Carts.Application;
using Carts.Domain.Aggregate;
using Senf.EventSourcing.Testing;

namespace Carts.TestingInfrastructure;

public class FakeShoppingCartRepository
    : InMemoryAggregateRepository<Guid, ShoppingCart>,
      IShoppingCartRepository
{
}
