using System;
using Carts.Domain;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.EventStore;

namespace Carts;

public interface ICartRepository : IAggregateRepository<Guid, CartAggregate>;

public class CartRepository
    : AggregateRepository<Guid, CartAggregate>,
      ICartRepository
{
    public CartRepository(
        IStreamReader streamReader,
        IStreamWriter streamWriter)
        : base(streamReader, streamWriter)
    {
    }

    public override string StreamPrefix => "cart-";
}
