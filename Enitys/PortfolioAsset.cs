using System;
using System.Collections.Generic;

namespace FinScope.Enitys;

public partial class PortfolioAsset
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int StockId { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? AvgPrice { get; set; }

    public decimal? CurrentPrice { get; set; }

    public decimal? Value { get; set; }

    public decimal? Profit { get; set; }

    public decimal? ProfitPercent { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
