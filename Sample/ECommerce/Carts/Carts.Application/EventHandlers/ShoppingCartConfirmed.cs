using System.Collections.Generic;
using System.Linq;
using Carts.Domain.Aggregate;
using Carts.Domain.Aggregate.Events;
using Carts.Domain.Model;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.EventSourcing.Core.Events;
using Kuna.EventSourcing.Core.Ids;

namespace Carts.Application.EventHandlers;

public record CartFinalized(
    GuidId CartId,
    GuidId ClientId,
    IReadOnlyList<PricedProductItem> ProductItems,
    decimal TotalPrice,
    DateTime FinalizedAt)
{
    public static CartFinalized Create(
        Guid cartId,
        Guid clientId,
        IReadOnlyList<PricedProductItem> productItems,
        decimal totalPrice,
        DateTime finalizedAt)
    {
        return new CartFinalized(cartId, clientId, productItems, totalPrice, finalizedAt);
    }
}

public class ShoppingCartConfirmedHandler : IHandleEvent<ShoppingCartConfirmed>
{
    private readonly IAggregateStreamReader streamReader;

    public ShoppingCartConfirmedHandler(IAggregateStreamReader streamReader)
    {
        this.streamReader = streamReader;
    }

    public async Task Handle(ShoppingCartConfirmed @event, CancellationToken ct)
    {
        // This would very much depend on implementation details of how
        // we want to distribute integration events, and whether meta-data should be present
        // to many assumptions here are made. Maybe we are using Pulsar, Kafka, EventHub, RabbitMQ, Azure ASB  etc.

        var events = await this.streamReader.GetEvents("cart-" + @event.CartId, ct);
        var state = new ShoppingCart.State();
        state.InitWith(events);

        // this could be pushed into ASB, Pulsar, Kafka, RabbitMQ etc.
        var cartFinalized =
            CartFinalized.Create(
                @event.CartId,
                state.ClientId,
                state.ProductItems.ToList(),
                state.TotalPrice,
                @event.ConfirmedAt
            );

        // await eventBus.Publish(cartFinalized, cancellationToken);
    }
}
