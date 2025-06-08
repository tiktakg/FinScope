using System;
using System.Collections.Generic;

namespace FinScope.Enitys;

public partial class SectorAllocation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Sector { get; set; }

    public decimal? Value { get; set; }

    public decimal? Percentage { get; set; }

    public virtual User User { get; set; } = null!;
}
