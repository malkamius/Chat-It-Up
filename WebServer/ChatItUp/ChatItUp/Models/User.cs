using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? DisplayName { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string? Status { get; set; }

    public virtual ICollection<UserActivePrivateChannel> UserActivePrivateChannels { get; set; } = new List<UserActivePrivateChannel>();

    public virtual ICollection<UserServer> UserServers { get; set; } = new List<UserServer>();

    public virtual ICollection<Channel> Channels { get; set; } = new List<Channel>();
}
