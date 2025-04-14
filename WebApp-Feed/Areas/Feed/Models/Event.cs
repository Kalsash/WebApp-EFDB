using System;
using System.Collections;
using System.Collections.Generic;
using WebApp_Feed.Areas.Feed.Models;

namespace WebApp_Feed.Models;
public partial class Event : IFeedItem
{
    public long EventId { get; set; }

    public long? PostId { get; set; }

    public byte[] EventTime { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? HostOrg { get; set; }

    public long? RsvpCount { get; set; }

    public long? MaxCapacity { get; set; }

    public virtual Post? Post { get; set; }

    DateTime IFeedItem.CreatedAt => DateTime.FromBinary(BitConverter.ToInt64(EventTime, 0));
}
