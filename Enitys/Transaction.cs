using System;
using System.Collections.Generic;

namespace FinScope.Enitys;

public partial class Transaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int StockId { get; set; }

    public DateTime Date { get; set; }

    public string? Type { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Total { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
