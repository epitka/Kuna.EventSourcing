using System;
using Senf.EventSourcing.Core.Events;

namespace Carts.Events;

public record ShoppingCartCanceled(GuidId CartId, DateTime CanceledAt) : IAggregateEvent;


