using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Client
{
    public long UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Address1 { get; set; } = null!;

    public string? Address2 { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? ZipCode { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
