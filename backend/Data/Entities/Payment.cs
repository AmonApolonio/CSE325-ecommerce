using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Payment
{
    public long PaymentId { get; set; }

    public long OrderId { get; set; }

    public DateOnly PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
