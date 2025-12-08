using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Product
{
    public long ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Price { get; set; }

    public decimal Inventory { get; set; }

    public string? Url { get; set; }

    public long CategoryId { get; set; }

    public long SellerId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrdersProduct> OrdersProducts { get; set; } = new List<OrdersProduct>();

    public virtual Seller? Seller { get; set; } = null!;
}
