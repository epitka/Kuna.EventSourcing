using Carts.Domain.Aggregate.Events;
using Carts.Domain.Model;
using Kuna.EventSourcing.Core.Aggregates;
using Kuna.Utilities.Extensions;

namespace Carts.Domain.Aggregate;

public partial class ShoppingCart
{
    public class State : AggregateState<Guid>
    {
        public Guid ClientId { get; set; }

        public ShoppingCartStatus Status { get; set; }

        public List<PricedProductItem> ProductItems { get; set; } = new();

        public decimal TotalPrice => this.ProductItems.Sum(pi => pi.TotalPrice);

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

        public void Apply(ShoppingCartProductAdded @event)
        {
            var newProductItem = PricedProductItem.Create(@event.ProductId, @event.Quantity, @event.UnitPrice);

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

        public void Apply(ShoppingCartProductRemoved @event)
        {
            var productItemToBeRemoved = PricedProductItem.Create(@event.ProductId, @event.Quantity, @event.UnitPrice);

            var existingProductItem = this.FindProductItemMatchingWith(productItemToBeRemoved);

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
