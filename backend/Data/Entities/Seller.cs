using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Seller
{
    public long SellerId { get; set; }

    public string Name { get; set; } = null!;

    public string PhotoUrl { get; set; } = null!;

    public string? AboutMe { get; set; }

    public string Address1 { get; set; } = null!;

    public string? Address2 { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public decimal CommisionRate { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
