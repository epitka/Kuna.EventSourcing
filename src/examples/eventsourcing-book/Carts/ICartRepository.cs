using System;
using Kuna.EventSourcing.Core.Aggregates;

namespace Carts;

public interface ICartRepository : IAggregateRepository<Guid, Domain.CartAggregate>;

