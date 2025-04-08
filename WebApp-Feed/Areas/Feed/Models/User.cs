using System;
using System.Collections.Generic;

namespace WebApp_Feed.Models;

public partial class User
{
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public byte[]? CreatedAt { get; set; }

    public byte[]? IsActive { get; set; }

    public virtual Auth? Auth { get; set; }

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
