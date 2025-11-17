using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class OrdersProduct
{
    public long OrdersOrderId { get; set; }

    public long ProductsProductId { get; set; }

    public decimal Quantity { get; set; }

    public virtual Order OrdersOrder { get; set; } = null!;

    public virtual Product ProductsProduct { get; set; } = null!;
}
