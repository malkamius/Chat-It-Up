using System;
using System.Collections.Generic;

namespace ChatItUp.Models;

public partial class UserActivePrivateChannel
{
    public Guid UserId { get; set; }

    public Guid ChannelId { get; set; }

    public DateTime LastActivity { get; set; }

    public virtual Channel Channel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
