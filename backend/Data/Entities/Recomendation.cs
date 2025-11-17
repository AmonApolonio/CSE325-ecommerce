using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class Recomendation
{
    public long RecommendationId { get; set; }

    public long ClientId { get; set; }

    public long ProductId { get; set; }

    public int Rating { get; set; }

    public string? Commentary { get; set; }
}
