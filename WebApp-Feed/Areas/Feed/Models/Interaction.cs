using System;
using System.Collections.Generic;

namespace WebApp_Feed.Models;

public partial class Interaction
{
    public long InteractionId { get; set; }

    public long UserId { get; set; }

    public long PostId { get; set; }

    public string InteractionType { get; set; } = null!;

    public byte[]? CreatedAt { get; set; }

    public string? Content { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
