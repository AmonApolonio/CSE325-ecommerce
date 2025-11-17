using System;
using System.Collections.Generic;

namespace backend.Data.Entities;

public partial class RecomendationsClient
{
    public long RecomendationsUserId { get; set; }

    public long ClientsUserId { get; set; }

    public virtual Client ClientsUser { get; set; } = null!;
}
