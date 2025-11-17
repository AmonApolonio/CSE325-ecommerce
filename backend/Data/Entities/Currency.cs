using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Currency
{
    public string CurrencyCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public double? ExchangeRateToBrl { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
