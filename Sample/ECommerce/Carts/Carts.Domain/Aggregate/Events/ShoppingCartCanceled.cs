using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;

namespace Carts.Domain.Aggregate.Events;

public record ShoppingCartCanceled(Guid CartId, DateTime CanceledAt) : IAggregateEvent;


