using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class RecomendationsProduct
{
    public long RecomendationsProductId { get; set; }

    public long ProductsProductId { get; set; }

    public virtual Product ProductsProduct { get; set; } = null!;
}
