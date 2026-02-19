using System;
using System.Collections.Generic;
using Carts.Domain.Models;
using Kuna.EventSourcing.Core.Aggregates;

namespace Carts.Domain;

public partial class CartAggregate
{
    public class State : AggregateState<Guid>
    {
        public Dictionary<CartItemId, ProductId> CartItems = [];

        public Dictionary<ProductId, double> ProductPrice = [];

        public bool Submitted = false;

        public bool Published { get; set; }

        public bool PublicationFailed { get; set; }


        public void Apply(CartEvents.CartCreated @event)
        {
            this.SetId(@event.CartId);
        }

        public void Apply(CartEvents.ItemAdded @event)
        {
            this.CartItems[@event.ItemId] = @event.ProductId;
            this.ProductPrice[@event.ProductId] = @event.Price;
        }

        public void Apply(CartEvents.ItemRemoved @event)
        {
            this.CartItems.Remove(@event.ItemId);
            this.ProductPrice.Remove(this.CartItems[@event.ItemId]);
        }

        public void Apply(CartEvents.CartCleared @event)
        {
            this.CartItems.Clear();
            this.ProductPrice.Clear();
        }

        public void Apply(CartEvents.ItemArchived @event)
        {
            this.CartItems.Remove(@event.ItemId);
            this.ProductPrice.Remove(this.CartItems[@event.ItemId]);
        }

        public void Apply(CartEvents.CartSubmitted @event)
        {
            this.Submitted = true;
        }

        public void Apply(CartEvents.CartPublished @event)
        {
            this.Published = true;
        }

        public void Apply(CartEvents.CartPublicationFailed @event)
        {
            this.PublicationFailed = true;
        }
    }
}
