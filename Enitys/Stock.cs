using System;
using System.Collections.Generic;

namespace FinScope.Enitys;

public partial class Stock
{
    public int Id { get; set; }

    public string Symbol { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public decimal? Price { get; set; }

    public decimal? Change { get; set; }

    public decimal? ChangePercent { get; set; }

    public long? Volume { get; set; }

    public string? Sector { get; set; }

    public virtual ICollection<PortfolioAsset> PortfolioAssets { get; set; } = new List<PortfolioAsset>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public string ChangeColor => ChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
}


