using System;
using System.Collections.Generic;

namespace FinScope.Enitys;

public partial class MarketIndex
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? Value { get; set; }

    public decimal? Change { get; set; }

    public decimal? ChangePercent { get; set; }

    public string ChangeColor => ChangePercent >= 0 ? "#FF4CAF50" : "#FFF44336";
}
