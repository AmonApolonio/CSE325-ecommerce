using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Category
{
    public long CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
