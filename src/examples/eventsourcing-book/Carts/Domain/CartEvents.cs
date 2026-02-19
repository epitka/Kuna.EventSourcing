using System;
using System.Collections.Immutable;
using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Domain;

public class CartEvents
{
    public readonly record struct CartCreated(Guid CartId) : IAggregateEvent;

    public readonly record struct ItemAdded(
        Guid CartId,
        string Description,
        string Image,
        double Price,
        Guid ItemId,
        Guid ProductId,
        string DeviceFingerPrint = Fingerprint.Default // provides default values for old events so we do not have to version events
    ) : IAggregateEvent;

    public readonly record struct InventoryChanged(
        Guid ProductId,
        int Inventory) : IAggregateEvent;

    public readonly record struct PriceChanged(
        Guid ProductId,
        decimal NewPrice,
        decimal OldPrice) : IAggregateEvent;

    public readonly record struct ItemArchived(
        Guid CartId,
        Guid ItemId) : IAggregateEvent;

    public readonly record struct ItemRemoved(
        Guid CartId,
        Guid ItemId) : IAggregateEvent;

    public readonly record struct CartSubmitted(
        Guid CartId,
        ImmutableArray<CartSubmitted.OrderedProduct> OrderedProducts,
        double TotalPrice) : IAggregateEvent
    {
        public readonly record struct OrderedProduct(Guid ProductId, double Price);
    }

    public readonly record struct CartCleared(Guid CartId) : IAggregateEvent;

    public readonly record struct CartPublicationFailed(Guid CartId) : IAggregateEvent;

    public readonly record struct CartPublished(Guid CartId) : IAggregateEvent;
}
