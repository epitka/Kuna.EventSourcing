
using Senf.EventSourcing.Core.Events;

namespace Carts.Events;

public record ShoppingCartOpened(GuidId CartId, GuidId ClientId) : IAggregateEvent;
