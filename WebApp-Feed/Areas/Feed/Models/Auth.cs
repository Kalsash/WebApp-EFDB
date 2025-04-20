using System;
using System.Collections.Generic;

namespace WebApp_Feed.Models;

public partial class Auth
{
    public long UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public byte[]? LastLogin { get; set; }

    public string? ResetToken { get; set; }

    public byte[]? TokenExpiry { get; set; }

    // Новые поля
    public string NormalizedUsername { get; set; }
    public string SecurityStamp { get; set; }
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    public virtual User User { get; set; } = null!;
}
