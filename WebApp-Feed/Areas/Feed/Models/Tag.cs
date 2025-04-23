using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp_Feed.Models;

public partial class Tag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long TagId { get; set; }

    public string TagName { get; set; } = null!;

    public byte[]? CreatedAt { get; set; }

    public long? UsageCount { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
