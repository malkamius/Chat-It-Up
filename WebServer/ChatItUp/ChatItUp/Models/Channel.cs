using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class Channel
{
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public Guid? ServerId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? DeletedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<UserActivePrivateChannel> UserActivePrivateChannels { get; set; } = new List<UserActivePrivateChannel>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
