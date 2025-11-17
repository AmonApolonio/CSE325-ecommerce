using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Order
{
    public long OrderId { get; set; }

    public long ClientId { get; set; }

    public decimal SubTotalCents { get; set; }

    public decimal TaxCents { get; set; }

    public decimal? FreightCents { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public virtual Currency CurrencyCodeNavigation { get; set; } = null!;

    public virtual ICollection<OrdersProduct> OrdersProducts { get; set; } = new List<OrdersProduct>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
