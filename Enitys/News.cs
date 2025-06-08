using System;
using System.Collections.Generic;

namespace FinScope.Enitys;

public partial class News
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Url { get; set; }

    public string? Source { get; set; }

    public DateTime PublishedAt { get; set; }
}
