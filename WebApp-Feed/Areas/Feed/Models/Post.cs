using System;
using System.Collections.Generic;
using WebApp_Feed.Areas.Feed.Models;

namespace WebApp_Feed.Models;

public partial class Post :IFeedItem
{
    public long PostId { get; set; }

    public long UserId { get; set; }

    public string Content { get; set; } = null!;

    public string PostType { get; set; } = null!;

    public string? MediaUrl { get; set; }

    public string? MediaType { get; set; }

    public string? AltText { get; set; }

    public string? ThumbnailUrl { get; set; }

    public byte[]? CreatedAt { get; set; }

    public long? ParentPostId { get; set; }

    public virtual Event? Event { get; set; }

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();

    public virtual ICollection<Post> InverseParentPost { get; set; } = new List<Post>();

    public virtual Post? ParentPost { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    DateTime IFeedItem.CreatedAt => DateTime.FromBinary(BitConverter.ToInt64(CreatedAt, 0));
}
