using System;
using System.Collections.Generic;

namespace WebApp_Feed.Models;

public partial class Event
{
    public long EventId { get; set; }

    public long? PostId { get; set; }

    public byte[] EventTime { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? HostOrg { get; set; }

    public long? RsvpCount { get; set; }

    public long? MaxCapacity { get; set; }

    public virtual Post? Post { get; set; }
}
