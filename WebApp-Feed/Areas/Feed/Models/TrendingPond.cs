using System;
using System.Collections.Generic;

namespace WebApp_Feed.Models;

public partial class TrendingPond
{
    public long? TagId { get; set; }

    public string? TagName { get; set; }

    public byte[]? RecentPosts { get; set; }
}
