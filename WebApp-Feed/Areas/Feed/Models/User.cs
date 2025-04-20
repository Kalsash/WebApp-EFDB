using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp_Feed.Models;

public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    public byte[]? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual Auth? Auth { get; set; }

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

}
