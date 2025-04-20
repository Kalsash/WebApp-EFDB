using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WebApp_Feed.Models;

public partial class Auth : IdentityUser<long>  // Id уже есть здесь (тип long)
{
    public byte[]? LastLogin { get; set; }
    public string? ResetToken { get; set; }
    public byte[]? TokenExpiry { get; set; }

    public virtual User User { get; set; } = null!;  // Связь с User
}