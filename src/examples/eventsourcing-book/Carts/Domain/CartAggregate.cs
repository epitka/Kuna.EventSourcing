using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Carts.Domain.Commands;
using Carts.Domain.Models;
using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Domain;

public partial class CartAggregate : Aggregate<Guid, CartAggregate.State>
{
    // this ctor should never be used from withing code, it is here to support restoring state
    public CartAggregate()
    {
    }

    // original book implementation does not have CreateCart command
    // method that "creates" aggregate is always static
    public static CartAggregate Process(CreateCart command)
    {
        return new CartAggregate(command.CartId);
    }

    // TODO: we should have Fody based check here to see if command belongs to this aggregate
    // TODO: use option that returns Result with error message rather then throwing an exception
    public void Handle(
        AddItem command,
        Fingerprint fingerprint)
    {
        // this is in original implementation, but with introduction of CreateCart command this check
        // is not necessary
        /*if (this.Id.Value == Guid.Empty)
        {
            var @event = new CartEvents.CartCreated(command.CartId);

            this.RaiseEvent(@event);
        }*/

        if (this.CurrentState.CartItems.Count >= 3)
        {
            throw new ValidationException("can only add 3 items");
        }

        var itemAdded = new CartEvents.ItemAdded(
            CartId: command.CartId,
            Description: command.Description,
            Image: command.Image,
            Price: command.Price,
            ProductId: command.ProductId,
            ItemId: command.ItemId,
            DeviceFingerPrint: fingerprint.Calculate()
        );

        this.RaiseEvent(itemAdded);
    }

    public void Handle(RemoveItem command)
    {
        if (this.CurrentState.CartItems.ContainsKey(command.ItemId) == false)
        {
            throw new ValidationException("Item {command.itemId} not in the Cart");
        }

        this.RaiseEvent(
            new CartEvents.ItemRemoved(command.CartId, command.ItemId)
        );
    }

    public void Handle(ClearCart command)
    {
        this.RaiseEvent(
            new CartEvents.CartCleared(command.CartId)
        );
    }

    public void Handle(ArchiveItem command)
    {
        var item = this.CurrentState.CartItems
                       .FirstOrDefault(x => (Guid)x.Value == command.ProductId);

        if (item.Value != null)
        {
            this.RaiseEvent(
                new CartEvents.ItemArchived(command.CartId, item.Key)
            );
        }
    }

    public void Handle(SubmitCart command)
    {
        var cartItems = this.CurrentState.CartItems;

        if (cartItems.Count == 0)
        {
            throw new ValidationException("cannot submit empty cart");
        }

        if (this.CurrentState.Submitted)
        {
            throw new ValidationException("cannot submit a cart twice");
        }

        var orderedProducts = cartItems
                              .Select(x => new CartEvents.CartSubmitted.OrderedProduct(
                                          x.Value,
                                          this.CurrentState.ProductPrice[x.Value]!))
                              .ToImmutableArray();

        var totalPrice = cartItems
                         .Select(x => this.CurrentState.ProductPrice[x.Value]!)
                         .Sum();

        this.RaiseEvent(
            new CartEvents.CartSubmitted(
                command.CartId,
                orderedProducts,
                totalPrice
            )
        );
    }

    /*fun publish() {
        if (!this.submitted) {
            throw CommandException("cannot publish unsubmitted cart")
        }
        if (this.published) {
            throw CommandException("cannot publish cart twice")
        }
        AggregateLifecycle.apply(CartPublishedEvent(this.aggregateId!!))
    }

    fun failPublication() {
        AggregateLifecycle.apply(CartPublicationFailedEvent(this.aggregateId!!))
    }*/

    private CartAggregate(Guid cartId)
    {
        var @event = new CartEvents.CartCreated(cartId);

        this.RaiseEvent(@event);
    }
}
