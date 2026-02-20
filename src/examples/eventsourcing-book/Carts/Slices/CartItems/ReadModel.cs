using System;
using System.Collections.Generic;
using System.Linq;
using Carts.Domain;

namespace Carts.Slices.CartItems;

public class ReadModel
{
    public Guid CartId { get; set; }
    public double TotalPrice { get; set; }
    public List<CartItem> Items { get; } = [];

    public record struct CartItem(
        Guid ItemId,
        Guid CartId,
        string Description,
        string Image,
        double Price,
        Guid ProductId,
        string FingerPrint);

    public void Apply(CartEvents.CartCreated e)
    {
        this.CartId = e.CartId;
    }

    public void Apply(CartEvents.ItemAdded e)
    {
        this.Items.Add(
            new CartItem(
                ItemId: e.ItemId,
                CartId: e.CartId,
                Description: e.Description,
                Image: e.Image,
                Price: e.Price,
                ProductId: e.ProductId,
                FingerPrint: e.DeviceFingerPrint
            ));

        this.TotalPrice += e.Price;
    }

    public void Apply(CartEvents.ItemRemoved e)
    {
        var item = this.Items.First(x => x.ItemId == e.ItemId);
        this.TotalPrice -= item.Price;
        this.Items.Remove(item);
    }
}
