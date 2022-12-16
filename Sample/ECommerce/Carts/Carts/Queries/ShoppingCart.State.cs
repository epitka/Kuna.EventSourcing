using System;
using System.Collections.Generic;
using System.Linq;
using Carts.Events;
using Carts.ShoppingCarts.Products;
using Senf.EventSourcing.Core.Extensions;

namespace Carts.ShoppingCarts;

public partial class ShoppingCart
{
    public class State : Senf.EventSourcing.Core.Aggregates.AggregateState<Guid>
    {
        public Guid ClientId { get; set; }

        public ShoppingCartStatus Status { get; set; }

        public List<PricedProductItem> ProductItems { get; set; }

        public decimal TotalPrice => this.ProductItems.Sum(pi => pi.TotalPrice);

        // TODO: why do we need query method on the aggregate
        public PricedProductItem? FindProductItemMatchingWith(PricedProductItem productItem)
        {
            return this.ProductItems
                       .SingleOrDefault(pi => pi.MatchesProductAndPrice(productItem));
        }

        public void Apply(ShoppingCartOpened @event)
        {
            this.SetId(@event.CartId);

            this.ClientId = @event.ClientId;
            this.ProductItems = new List<PricedProductItem>(4);
            this.Status = ShoppingCartStatus.Pending;
        }

        public void Apply(ProductAdded @event)
        {
            var newProductItem = @event.ProductItem;

            var existingProductItem = this.FindProductItemMatchingWith(newProductItem);

            if (existingProductItem is null)
            {
                this.ProductItems.Add(newProductItem);
                return;
            }

            this.ProductItems.Replace(
                existingProductItem,
                existingProductItem.MergeWith(newProductItem)
            );
        }

        public void Apply(ProductRemoved @event)
        {
            var productItemToBeRemoved = @event.ProductItem;

            var existingProductItem = this.FindProductItemMatchingWith(@event.ProductItem);

            if (existingProductItem == null)
            {
                return;
            }

            if (existingProductItem.HasTheSameQuantity(productItemToBeRemoved))
            {
                this.ProductItems.Remove(existingProductItem);
                return;
            }

            this.ProductItems.Replace(
                existingProductItem,
                existingProductItem.Subtract(productItemToBeRemoved)
            );
        }

        public void Apply(ShoppingCartConfirmed @event)
        {
            this.Status = ShoppingCartStatus.Confirmed;
        }

        public void Apply(ShoppingCartCanceled @event)
        {
            this.Status = ShoppingCartStatus.Canceled;
        }
    }
}
