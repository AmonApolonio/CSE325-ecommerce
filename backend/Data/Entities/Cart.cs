using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Cart
{
    public long CartId { get; set; }

    public long UserId { get; set; }

    public DateOnly CreatedDate { get; set; }

    public DateOnly? UpdatedDate { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Client User { get; set; } = null!;
}
