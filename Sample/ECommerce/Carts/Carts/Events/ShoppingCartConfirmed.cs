using System;
using Senf.EventSourcing.Core.Events;

namespace Carts.Events;

public record ShoppingCartConfirmed(GuidId CartId, DateTime ConfirmedAt) : IAggregateEvent;