using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class ProductImage
{
    public long ProductImageId { get; set; }

    public long ProductId { get; set; }

    public string Url { get; set; } = null!;

    public string Alt { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
